namespace NBTerminal;

public static class Terminal
{
    private static volatile bool _inputEnabled = false;
    private static volatile bool _outputEnabled = false;

    public static bool InputEnabled => _inputEnabled;
    public static bool OutputEnabled => _outputEnabled;
    public static bool Enabled => _inputEnabled || _outputEnabled;

    private static volatile Exception? _lastError;
    public static Exception? LastError => _lastError;

    private static readonly Dictionary<string, Page> _pages = [];
    private static volatile string? _selectedPage;
    public static IReadOnlyDictionary<string, Page> Pages => _pages;
    public static string? CurrentPage => _selectedPage;

    public static bool RegisterPage(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!_pages.TryAdd(name, new Page())) return false;

        _selectedPage ??= name;
        return true;
    }
    public static bool SelectPage(string name)
    {
        if (!_pages.ContainsKey(name)) return false;

        _selectedPage = name;
        return true;
    }

    private static ConsoleKeyInfo? _inputKeyInfo;
    private static char? _inputChar;
    private static volatile string _inputString = string.Empty;
    public static ConsoleKeyInfo? InputKeyInfo => _inputKeyInfo;
    public static char? InputChar => _inputChar;
    public static string InputString => _inputString;

    public static event Action<ConsoleKeyInfo>? InputReadKey;
    public static event Action<char>? InputRead;
    public static event Action<string>? InputReadLine;
    public static event Action<Frame>? Output;
    public static event Action<Exception>? Error;

    private static string[] _previousFrame = [];
    private static int _previousWidth;
    private static int _previousHeight;

    private static void DrawFrame(string content)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        if (width <= 0 || height <= 0) return;

        if (width != _previousWidth || height != _previousHeight)
        {
            Console.Clear();

            _previousFrame = [];
            _previousWidth = width;
            _previousHeight = height;
        }

        string[] lines = content
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n');

        string[] currentFrame = new string[height];

        for (int y = 0; y < height; y++)
        {
            int maxWidth = width - (y == height - 1 ? 1 : 0);

            string current = y < lines.Length ? lines[y] : "";
            string previous = y < _previousFrame.Length ? _previousFrame[y] : "";

            if (current.Length > maxWidth) current = current[..maxWidth];

            currentFrame[y] = current;

            if (current == previous) continue;

            Console.SetCursorPosition(0, y);
            Console.Write(current);

            if (current.Length < previous.Length)
            {
                Console.Write(new string(' ', previous.Length - current.Length));
            }
        }

        _previousFrame = currentFrame;
    }

    private static Task? _inputTask;
    private static Task? _outputTask;

    private static async Task InputLoop()
    {
        try
        {
            while (_inputEnabled)
            {
                while (_inputEnabled)
                {
                    ConsoleKeyInfo key;

                    try
                    {
                        if (!Console.KeyAvailable) break;

                        key = Console.ReadKey(true);
                    }
                    catch (IOException ex)
                    {
                        OnError(ex);
                        break;
                    }
                    catch (InvalidOperationException ex)
                    {
                        OnError(ex);
                        throw;
                    }

                    string? selectedPage = _selectedPage;
                    Page? page = null;
                    if (selectedPage is not null) _pages.TryGetValue(selectedPage, out page);

                    try
                    {
                        _inputKeyInfo = key;

                        InputReadKey?.Invoke(key);
                        page?.OnInputReadKey(key);

                        if (key.Key == ConsoleKey.Enter)
                        {
                            string line = _inputString;
                            _inputString = string.Empty;

                            InputReadLine?.Invoke(line);
                            page?.OnInputReadLine();
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (_inputString.Length > 0) _inputString = _inputString[..^1];
                            page?.OnBackspace();
                        }
                        else if (!char.IsControl(key.KeyChar))
                        {
                            _inputChar = key.KeyChar;
                            _inputString += key.KeyChar;

                            InputRead?.Invoke(key.KeyChar);
                            page?.OnInputRead(key.KeyChar);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                        throw;
                    }
                }

                if (_inputEnabled) await Task.Delay(10);
            }
        }
        finally
        {
            _inputEnabled = false;
        }
    }
    private static async Task OutputLoop()
    {
        Frame frame = new();
        bool previousCursorVisible = true;

        try
        {
            if (OperatingSystem.IsWindows()) previousCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            while (_outputEnabled)
            {
                frame.Clear();

                try
                {
                    Output?.Invoke(frame);

                    string? selectedPage = _selectedPage;
                    if (selectedPage is not null && _pages.TryGetValue(selectedPage, out Page? page))
                    {
                        page.OnOutput(frame);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    throw;
                }

                try
                {
                    DrawFrame(frame.ToString());
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _previousFrame = [];
                    _previousWidth = 0;
                    _previousHeight = 0;

                    OnError(ex);
                }
                catch (IOException ex)
                {
                    OnError(ex);
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    OnError(ex);
                    throw;
                }

                await Task.Delay(10);
            }
        }
        finally
        {
            try
            {
                Console.CursorVisible = previousCursorVisible;
            }
            finally
            {
                _outputEnabled = false;
            }
        }
    }

    public static void Start()
    {
        StartInput();
        StartOutput();
    }
    public static void StopRequest()
    {
        StopInputRequest();
        StopOutputRequest();
    }
    public static async Task StopAsync()
    {
        StopRequest();
        await Task.WhenAll(_inputTask ?? Task.CompletedTask, _outputTask ?? Task.CompletedTask);
    }

    public static void StartInput()
    {
        if (_inputEnabled) return;
        if (_inputTask is { IsCompleted: false }) throw new InvalidOperationException("Previous InputLoop has not yet completed");

        _inputEnabled = true;
        _inputTask = Task.Run(InputLoop);
    }
    public static void StopInputRequest()
    {
        _inputEnabled = false;
    }
    public static async Task StopInputAsync()
    {
        StopInputRequest();
        await Task.WhenAll(_inputTask ?? Task.CompletedTask);
    }

    public static void StartOutput()
    {
        if (_outputEnabled) return;
        if (_outputTask is { IsCompleted: false }) throw new InvalidOperationException("Previous OutputLoop has not yet completed");

        _outputEnabled = true;

        _previousFrame = [];
        _previousWidth = 0;
        _previousHeight = 0;
        _outputTask = Task.Run(OutputLoop);
    }
    public static void StopOutputRequest()
    {
        _outputEnabled = false;
    }
    public static async Task StopOutputAsync()
    {
        StopOutputRequest();
        await Task.WhenAll(_outputTask ?? Task.CompletedTask);
    }

    private static void OnError(Exception ex)
    {
        _lastError = ex;
        Error?.Invoke(ex);
    }
}
