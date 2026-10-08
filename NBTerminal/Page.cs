namespace NBTerminal;

public class Page
{
    private ConsoleKeyInfo? _inputKeyInfo;
    private char? _inputChar;
    private volatile string _inputString = string.Empty;

    public ConsoleKeyInfo? InputKeyInfo => _inputKeyInfo;
    public char? InputChar => _inputChar;
    public string InputString => _inputString;

    public event Action<ConsoleKeyInfo>? InputReadKey;
    public event Action<char>? InputRead;
    public event Action<string>? InputReadLine;
    public event Action<Frame>? Output;

    internal void OnInputReadKey(ConsoleKeyInfo obj)
    {
        _inputKeyInfo = obj;
        InputReadKey?.Invoke(obj);
    }
    internal void OnInputRead(char obj)
    {
        _inputChar = obj;
        _inputString += obj;

        InputRead?.Invoke(obj);
    }
    internal void OnInputReadLine()
    {
        string line = _inputString;
        _inputString = string.Empty;

        InputReadLine?.Invoke(line);
    }
    internal void OnBackspace()
    {
        if (_inputString.Length > 0) _inputString = _inputString[..^1];
    }
    internal void OnOutput(Frame frame)
    {
        Output?.Invoke(frame);
    }
}
