using System.Text;

namespace NBTerminal;

public sealed class Frame
{
    private readonly StringBuilder _buffer = new();

    public void Write(object? value)
    {
        _buffer.Append(value);
    }
    public void WriteLine(object? value = null)
    {
        _buffer.AppendLine(value?.ToString());
    }
    internal void Clear()
    {
        _buffer.Clear();
    }

    public override string ToString()
    {
        return _buffer.ToString();
    }
}
