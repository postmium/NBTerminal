# NBTerminal

**NBTerminal (Non-Blocking Terminal)** is a lightweight, event-driven library for building interactive console applications in **C# and .NET**.

It runs keyboard processing and screen updates in a background task, so the calling code does not have to manage a blocking input loop or manually repaint the console. Your application handles events and writes to a virtual screen; NBTerminal takes care of rendering the result.

NBTerminal focuses on a small API, predictable behavior, and no third-party dependencies rather than providing a complete collection of UI widgets.

## Features

- **Event-driven keyboard input:** receive keys, printable characters, and completed lines through C# events.
- **Independent input and output:** enable either subsystem, both, or neither while the main loop is running.
- **Pages:** separate input state and event handlers for different views of your application.
- **Virtual screen:** build a `Frame` with `Write()`, `WriteLine()`, `Set()`, and a virtual cursor.
- **Styled text:** use fluent text formatting, 256-color palettes, and 24-bit RGB foreground/background colors.
- **Composite styled text:** combine multiple independently styled fragments using `StyledText`, including plain strings and `StyledString` values.
- **Efficient updates:** reuse two frame buffers, compare their cells, and redraw only rows that changed.
- **Terminal control:** optional alternate screen, hidden cursor during rendering, and UTF-8 output.
- **Error reporting:** separate input, output, render, and fatal-loop exceptions.
- **No external dependencies:** built with the standard .NET libraries.

## Requirements

- **.NET 10** (`net10.0`).
- **Windows 10 or Windows 11** with an interactive VT/ANSI-compatible terminal. Windows Terminal is recommended.

Other VT-compatible terminals may work, but cross-platform behavior has not been fully verified. Windows 7 and 8 are not supported. NBTerminal is designed for an interactive console, not for redirected terminal output to files or pipes.

## Installation

Install NBTerminal from NuGet:

```bash
dotnet add package NBTerminal --version 0.2.1
```

Alternatively, add a reference to the NBTerminal project from your solution (in Visual Studio or with the .NET CLI):

```bash
dotnet add reference ../NBTerminal/NBTerminal.csproj
```

Adjust the path to match your solution structure.

## Quick start

This example creates two pages: a counter controlled by the arrow keys and a text-input page. Press **F1** or **F2** to switch between them, and **Escape** to exit.

```csharp
using NBTerminal;

// Create pages and choose the initial page.
Page counterPage = Terminal.Page("Counter");
Page textPage = Terminal.Page("Text");
Terminal.SelectPage("Counter");

int counter = 0;
string lastText = "(nothing submitted)";

// Create styled text once and reuse it during rendering.
StyledText formattingExample = new(
    "Formatting: ",
    "RGB".Bold().FgColor(255, 100, 100),
    " and ",
    "256-color palette".FgColor((byte)46).Underline()
);

// Global shortcuts work regardless of the selected page.
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
            Terminal.Stop();
            break;
    }
};

// Each page can handle its own keyboard input.
counterPage.InputReadKey += key =>
{
    if (key.Key == ConsoleKey.UpArrow) counter++;
    if (key.Key == ConsoleKey.DownArrow) counter--;
};

textPage.InputReadLine += line =>
{
    lastText = line;
};

// A page receives a fresh (or cleared and reused) frame on each update.
counterPage.Output += frame =>
{
    frame.WriteLine("NBTerminal 0.2.1".Bold().FgColor(80, 180, 255));
    frame.WriteLine("F1: Counter | F2: Text | Esc: Exit");
    frame.WriteLine();
    frame.WriteLine($"Counter: {counter}".FgColor(100, 255, 140));
    frame.WriteLine("Use Up / Down to change the value.");
    frame.WriteLine();
    frame.WriteLine(formattingExample);
};

textPage.Output += frame =>
{
    frame.WriteLine("NBTerminal 0.2.1".Bold().FgColor(80, 180, 255));
    frame.WriteLine("F1: Counter | F2: Text | Esc: Exit");
    frame.WriteLine();
    frame.WriteLine("Type something and press Enter.");
    frame.WriteLine($"Current input: {textPage.InputString}".FgColor(255, 220, 100));
    frame.WriteLine($"Last submitted: {lastText}");
};

// Input and output must be enabled explicitly.
Terminal.InputEnabled = true;
Terminal.OutputEnabled = true;

Terminal.Start(); // Starts the background loop and returns immediately.
Terminal.Wait();  // Wait only from the application's controlling code.

// The renderer has released the console by this point.
if (Terminal.MainException is { } error)
    Console.Error.WriteLine(error);
```

`Terminal.Page(key)` creates a page on first access or returns the existing one. `Terminal.SelectPage(key)` changes the active page without resetting its pending input.

### Input-only mode

You can use NBTerminal to handle keyboard input without taking control of the screen. In this mode, ordinary `Console.WriteLine()` calls are available:

```csharp
using NBTerminal;

Terminal.InputReadKey += key =>
{
    if (key.Key == ConsoleKey.Escape)
        Terminal.Stop();
    else
        Console.WriteLine($"Pressed: {key.Key}");
};

Terminal.InputEnabled = true;
Terminal.OutputEnabled = false;

Console.WriteLine("Press keys; Escape exits.");
Terminal.Start();
Terminal.Wait();
```

### Working with frames and styles

`Frame` is a virtual screen made up of `Cell` values. Every cell stores a `char` and its `Style`. Writing to a frame does **not** write directly to the console; the renderer processes the completed frame after the output handlers return.

```csharp
Terminal.Output += frame =>
{
    frame.WriteLine("Regular text");
    frame.WriteLine("Bold and underlined".Bold().Underline());
    frame.WriteLine("RGB foreground and background".FgColor(255, 220, 80).BgColor(40, 40, 100));
    frame.WriteLine("256-color palette".FgColor((byte)46));

    if (frame.Width > 0 && frame.Height > 0)
        frame.Set(frame.Width - 1, frame.Height - 1, new Cell('#', default));
};
```

Other available styles include italic, dim, strikethrough, overline, blinking, inverted colors, and concealed text. Whether a visual attribute is supported depends on the terminal.

`Frame.SetCursorPosition(x, y)` moves the **virtual** writing cursor. Writing wraps at the right edge and stops beyond the bottom edge; it does not scroll. Direct `Set()` calls replace individual cells and ignore out-of-range coordinates.

### Composite styled text

Starting with **NBTerminal 0.2.1**, `StyledText` allows multiple `StyledString` fragments to be combined into a single text object.

Unlike `StyledString`, which applies one style to its entire text, `StyledText` preserves the individual formatting of each fragment.

```csharp
StyledText text = new(
    "Regular text, ",
    "red text".FgColor(255, 0, 0),
    " and ",
    "bold green text".Bold().FgColor(0, 255, 0)
);

Terminal.Output += frame =>
{
    frame.WriteLine(text);
};
```

Each fragment retains its own foreground color, background color, and text attributes.

**Plain strings are supported.** `StyledString` provides an implicit conversion from `string`, allowing regular and styled strings to be mixed without explicit constructors.

```csharp
StyledText message = new(
    "Health: ",
    "75%".Bold().FgColor(0, 255, 0),
    " | ",
    "Warning!".Blink().FgColor(255, 191, 0)
);
```

#### Writing StyledText to a frame

`Frame` provides two overloads for composite styled text:

- `Frame.Write(StyledText)` — writes all fragments sequentially at the current virtual cursor position.
- `Frame.WriteLine(StyledText)` — writes all fragments and advances the virtual cursor to the next line.

Each fragment is processed through the existing `StyledString` writing mechanism. Colors and attributes are preserved independently, and the virtual cursor continues advancing across fragment boundaries.

#### Immutability and allocations

`StyledText` is an immutable collection of `StyledString` values.

- Fragments are copied into an internal array when the object is created.
- Individual fragments cannot be modified through `StyledText`.
- `StyledString` is a value type, so storing its values does not require a separate heap object for each fragment.
- Existing `StyledText` instances can be reused across multiple frames.
- Indexed iteration through fragments does not require allocating an enumerator.

For frequently updated interfaces, create reusable `StyledText` objects **outside** output event handlers whenever their contents remain unchanged.

```csharp
StyledText title = new(
    "NB".FgColor(0, 255, 0),
    "Terminal".Bold().FgColor(80, 180, 255)
);

Terminal.Output += frame =>
{
    frame.WriteLine(title);
};
```

This avoids rebuilding the collection of fragments on every screen update.

`StyledText` handles text composition rather than layout. It does not automatically align text, create panels, or manage screen regions.

## How it works

Each iteration of the background loop performs these operations sequentially:

1. Read available keyboard input and invoke input handlers, if enabled.
2. Start or stop control of the terminal screen when `OutputEnabled` changes.
3. Build a frame by invoking the selected page's `Output` handlers, then the global `Terminal.Output` handlers.
4. Compare the new frame against the last rendered frame and send only changed rows to the terminal.
5. Wait approximately 10 ms before the next iteration (in addition to the time spent processing events and output).

Two `Frame` instances are reused while the terminal dimensions remain unchanged. Rendering uses VT/ANSI control sequences and UTF-8 bytes sent to standard output. A size change or a new rendering session triggers a full redraw.

The library uses **one logical background loop**, not separate concurrent input and output loops. Event handlers run sequentially within that loop and should complete synchronously.

## Lifecycle and configuration

| API | Purpose |
| --- | --- |
| `Terminal.InputEnabled` | Enable or disable keyboard processing. |
| `Terminal.OutputEnabled` | Enable or disable screen rendering. |
| `Terminal.UseAlternateScreen` | Choose whether the *next* terminal session uses the alternate screen (default: `true`). |
| `Terminal.Start()` | Start the background loop without waiting for it to finish. |
| `Terminal.StartAsync()` | Asynchronously initiate/restart the loop; does not wait for its entire lifetime. |
| `Terminal.Stop()` | Request the loop to stop without waiting. Safe to call from event handlers. |
| `Terminal.Wait()` / `Terminal.WaitAsync()` | Wait for the current loop to finish without propagating its stored exception. |
| `Terminal.StopAsync()` | Request termination and asynchronously wait for cleanup. |
| `Terminal.Enabled` | Whether the loop is currently marked as running. |

**Important:** call waiting methods from your application's controlling code, not synchronously from a terminal event handler. Waiting for the current loop from one of its own handlers can cause a deadlock. Use `Terminal.Stop()` inside handlers and `Terminal.Wait()` or `Terminal.WaitAsync()` outside them.

Changing `OutputEnabled` is a request: initialization and cleanup take place on the next loop iteration, not immediately in the property setter. Changing `UseAlternateScreen` during a session takes effect only after stopping and starting the terminal again.

When output is active, avoid `Console.Write()` and `Console.WriteLine()` in other code: they are not coordinated with the renderer and can corrupt the displayed image.

## Error handling

NBTerminal records the latest error in each subsystem and exposes a corresponding event:

| Property | Event | Behavior |
| --- | --- | --- |
| `Terminal.InputException` | `Terminal.InputError` | A user input-handler failure is reported and the loop continues. Console input failures may disable input. |
| `Terminal.OutputException` | `Terminal.OutputError` | A frame-building failure is reported; that frame is skipped and later frames may still render. |
| `Terminal.RenderException` | `Terminal.RenderError` | A rendering failure is reported and output is disabled to prevent repeated failures. |
| `Terminal.MainException` | — | Contains a fatal loop error, including an exception thrown by an error-event handler. |

For example:

```csharp
Exception? lastInputError = null;
Exception? lastOutputError = null;

Terminal.InputError += error => lastInputError = error;
Terminal.OutputError += error => lastOutputError = error;

// Inspect Terminal.MainException after Terminal.Wait().
```

Errors inside `InputError`, `OutputError`, or `RenderError` handlers are fatal to the main loop. The waiting helpers do not rethrow loop errors; inspect `Terminal.MainException` after waiting. `Terminal.MainTask` remains available if you need to inspect the underlying task yourself.

## Limitations and design notes

- **Not fully thread-safe.** Treat terminal events and frame building as a single sequential workflow. Concurrent access to application state needs coordination by the application.
- **One process-wide terminal instance.** `Terminal` is a static class with shared state.
- **No automatic input echo.** Use `InputReadKey`, `InputRead`, and `InputReadLine` or the page-specific events to display entered text.
- **No automatic scrolling.** `Frame.Write()` stops when it runs past the bottom of the buffer.
- **Limited Unicode cell model.** Each cell stores a single `char`; wide characters, combining sequences, emoji represented by surrogate pairs, and grapheme-aware editing are not yet supported. Common one-column characters, including Cyrillic, are supported.
- **Interactive terminals only.** Redirected output and legacy terminal behavior are outside the intended use case.
- **No built-in widgets or layout engine.** Menus, windows, selection state, and other UI elements are built in the application's output handlers.
- **Event subscriptions and page state persist** between `Start()` / `Stop()` sessions unless your application changes them.
- **Terminal-dependent text attributes.** Blinking and other visual effects may be ignored or rendered differently depending on the terminal emulator.

## Contributing

Issues, bug reports, ideas, and pull requests are welcome. Please include the .NET version, terminal application, operating system, and reproduction steps when reporting rendering problems.

## License

NBTerminal is released under the [MIT License](LICENSE.txt).
