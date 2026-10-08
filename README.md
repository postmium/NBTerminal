# NBTerminal

**NBTerminal (Non-Blocking Terminal)** is a lightweight, event-driven terminal library for C# and .NET. It provides keyboard input through events and independently controlled, automatically refreshed console output.

The library is intended for small interactive console applications, command-line tools, and text-based interfaces without requiring a full terminal UI framework.

## Features

- **Non-blocking startup:** run keyboard input on a background task without blocking the calling code.
- **Independent input and output:** start and stop either subsystem separately or together.
- **Event-driven input:** handle keys, characters, and completed lines through C# events.
- **Automatic rendering:** build a frame with `Write()` and `WriteLine()`; only changed lines are redrawn.
- **Pages:** register pages with their own input events, output handlers, and text buffers.
- **Error reporting:** subscribe to `Terminal.Error` or inspect `Terminal.LastError`.
- **No third-party dependencies.**

## Requirements

- .NET 10 or later (the current package targets `net10.0`).
- An interactive console/terminal for keyboard input and dynamic rendering.

## Installation

Once NBTerminal is published to NuGet:

```bash
dotnet add package NBTerminal
```

Or reference the `NBTerminal` project directly when building from source.

## Examples

### 1. Keyboard input without blocking the main flow

```csharp
using NBTerminal;

Console.WriteLine("Press any key. Escape to exit.");

Terminal.InputReadKey += key =>
{
    if (key.Key == ConsoleKey.Escape)
        Terminal.StopInputRequest();
    else
        Console.WriteLine($"Pressed: {key.Key}");
};

Terminal.StartInput();

while (Terminal.InputEnabled)
    await Task.Delay(50);

await Terminal.StopInputAsync();
```

Here, NBTerminal handles keyboard polling. Standard `Console.WriteLine()` remains available because the rendering subsystem is not running.

### 2. Automatically updated console output

```csharp
using NBTerminal;

Terminal.Output += frame =>
{
    frame.WriteLine("NBTerminal");
    frame.WriteLine($"Current time: {DateTime.Now:HH:mm:ss}");
};

Terminal.StartOutput();

await Task.Delay(TimeSpan.FromSeconds(10));

await Terminal.StopOutputAsync();
```

The output handler is called repeatedly. NBTerminal compares frames and updates only lines that changed.

For an application that uses both input and output, call `Terminal.Start()` and stop it with `Terminal.StopRequest()` followed by `await Terminal.StopAsync()`.

See [`NBTerminal.Example`](NBTerminal.Example/) for a runnable example project.

## Important notes

- Input and output handlers run on background tasks. Synchronize application data accessed by both when necessary.
- Input is **not echoed automatically**. Use the `InputRead`, `InputReadLine`, and `InputReadKey` events to handle it.
- When `StartOutput()` is active, avoid writing directly to `Console`: doing so can interfere with the renderer.
- Register pages before starting the terminal. Concurrent changes to the page collection are not supported.
- The renderer is text-based and does not yet fully handle ANSI formatting or all Unicode character widths. Resizing the terminal may produce recoverable errors.
- Cross-platform terminal compatibility has not yet been fully tested.

## Contributing

Bug reports, suggestions, and pull requests are welcome. You can open an issue or propose a change through GitHub.

## License

NBTerminal is released under the [MIT License](LICENSE.txt).
