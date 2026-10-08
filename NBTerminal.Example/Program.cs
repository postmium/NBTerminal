using NBTerminal;

Terminal.RegisterPage("Counter");
Terminal.RegisterPage("Text");

Page counterPage = Terminal.Pages["Counter"];
Page textPage = Terminal.Pages["Text"];

int counter = 0;
string lastText = "None";

Terminal.InputReadKey += key =>
{
    switch (key.Key)
    {
        case ConsoleKey.F1:
            Terminal.SelectPage("Counter");
            break;

        case ConsoleKey.F2:
            Terminal.SelectPage("Text");
            break;

        case ConsoleKey.Escape:
            Terminal.StopRequest();
            break;
    }
};

counterPage.InputReadKey += key =>
{
    switch (key.Key)
    {
        case ConsoleKey.UpArrow:
            counter++;
            break;

        case ConsoleKey.DownArrow:
            counter--;
            break;
    }
};

textPage.InputReadLine += text =>
{
    Volatile.Write(ref lastText, text);
};

Terminal.Output += frame =>
{
    frame.WriteLine("=== NBTerminal Example ===");
    frame.WriteLine();
    frame.WriteLine("F1 - Counter | F2 - Text | Esc - Exit");
    frame.WriteLine($"Current page: {Terminal.CurrentPage}");
    frame.WriteLine();
};

counterPage.Output += frame =>
{
    frame.WriteLine("=== COUNTER ===");
    frame.WriteLine();
    frame.WriteLine($"Value: {counter}");
    frame.WriteLine();
    frame.WriteLine("Up Arrow   - Increment");
    frame.WriteLine("Down Arrow - Decrement");
};

textPage.Output += frame =>
{
    frame.WriteLine("=== TEXT INPUT ===");
    frame.WriteLine();
    frame.WriteLine("Type something and press Enter.");
    frame.WriteLine();
    frame.WriteLine($"Current input: {textPage.InputString}");
    frame.WriteLine($"Last submitted: {lastText}");
};

Terminal.Start();

try
{
    while (Terminal.Enabled) await Task.Delay(50);
}
finally
{
    await Terminal.StopAsync();
}

Console.WriteLine("NBTerminal stopped.");