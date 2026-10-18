
using System.Collections;
using System.Collections.Generic;

namespace NBTerminal;

/// <summary>
/// Представляет элемент составного текста: стилизованную строку или вложенный StyledText.
/// </summary>
public readonly struct StyledTextPart
{
    public StyledString String { get; }
    public StyledText? Nested { get; }
    public bool IsNested => Nested is not null;

    public StyledTextPart(StyledString value)
    {
        String = value;
        Nested = null;
    }

    public StyledTextPart(StyledText value)
    {
        Nested = value ?? throw new ArgumentNullException(nameof(value));
        String = default;
    }

    public static implicit operator StyledTextPart(string value) => new(new StyledString(value));
    public static implicit operator StyledTextPart(StyledString value) => new(value);
    public static implicit operator StyledTextPart(StyledText value) => new(value);
}

/// <summary>
/// Хранит составной текст с поддержкой вложенности и наследования стилей.
/// </summary>
/// <remarks>
/// Класс неизменяемый. Изменение оформления возвращает новый StyledText,
/// сохраняющий существующее дерево дочерних элементов.
/// </remarks>
public sealed class StyledText : IReadOnlyList<StyledTextPart>
{
    private readonly StyledTextPart[] _fragments;

    public int Count => _fragments.Length;
    public Style Style { get; }

    public StyledTextPart this[int index] => _fragments[index];

    /// <summary>
    /// Создаёт составной текст из строк и вложенных элементов.
    /// </summary>
    public StyledText(params StyledTextPart[] fragments) : this(fragments, default, true)
    {
    }

    private StyledText(StyledTextPart[] fragments, Style style, bool copy)
    {
        ArgumentNullException.ThrowIfNull(fragments);
        _fragments = copy ? (StyledTextPart[])fragments.Clone() : fragments;
        Style = style;
    }

    /// <summary>
    /// Возвращает новый составной текст с заданным общим стилем.
    /// </summary>
    public StyledText WithStyle(Style style) => new(_fragments, style, false);

    public StyledText ClearStyles() => WithStyle(Style.ClearStyles());
    public StyledText ClearTextStyles() => WithStyle(Style.ClearTextStyles());

    public StyledText Bold(bool enabled = true) => WithStyle(Style.Bold(enabled));
    public StyledText Dim(bool enabled = true) => WithStyle(Style.Dim(enabled));
    public StyledText Italic(bool enabled = true) => WithStyle(Style.Italic(enabled));
    public StyledText Underline(bool enabled = true) => WithStyle(Style.Underline(enabled));
    public StyledText Strikethrough(bool enabled = true) => WithStyle(Style.Strikethrough(enabled));
    public StyledText Overline(bool enabled = true) => WithStyle(Style.Overline(enabled));
    public StyledText Blink(bool enabled = true) => WithStyle(Style.Blink(enabled));
    public StyledText RapidBlink(bool enabled = true) => WithStyle(Style.RapidBlink(enabled));
    public StyledText Invert(bool enabled = true) => WithStyle(Style.Invert(enabled));
    public StyledText Conceal(bool enabled = true) => WithStyle(Style.Conceal(enabled));

    public StyledText FgDefault() => WithStyle(Style.FgDefault());
    public StyledText FgColor(byte index) => WithStyle(Style.FgColor(index));
    public StyledText FgColor(byte r, byte g, byte b) => WithStyle(Style.FgColor(r, g, b));

    public StyledText BgDefault() => WithStyle(Style.BgDefault());
    public StyledText BgColor(byte index) => WithStyle(Style.BgColor(index));
    public StyledText BgColor(byte r, byte g, byte b) => WithStyle(Style.BgColor(r, g, b));

    public IEnumerator<StyledTextPart> GetEnumerator()
    {
        return ((IEnumerable<StyledTextPart>)_fragments).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
