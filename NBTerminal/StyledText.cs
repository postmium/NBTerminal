
using System.Collections;
using System.Collections.Generic;

namespace NBTerminal;

/// <summary>
/// Хранит последовательность текстовых фрагментов с независимым оформлением.
/// </summary>
/// <remarks>
/// <para>
/// Фрагменты хранятся в исходном порядке и могут иметь разные цвета и текстовые атрибуты.
/// </para>
/// <para>
/// Класс неизменяемый. При создании копирует переданные фрагменты.
/// Самостоятельно не выполняет отрисовку.
/// </para>
/// </remarks>
public sealed class StyledText : IReadOnlyList<StyledString>
{
    private readonly StyledString[] _fragments;

    /// <summary>
    /// Возвращает количество текстовых фрагментов.
    /// </summary>
    public int Count => _fragments.Length;

    /// <summary>
    /// Возвращает фрагмент по индексу.
    /// </summary>
    /// <param name="index">Индекс фрагмента.</param>
    /// <returns>Стилизованная строка по указанному индексу.</returns>
    public StyledString this[int index] => _fragments[index];

    /// <summary>
    /// Создаёт текст из последовательности стилизованных строк.
    /// </summary>
    /// <param name="fragments">Фрагменты текста.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fragments"/> равен <see langword="null"/>.
    /// </exception>
    public StyledText(params StyledString[] fragments)
    {
        ArgumentNullException.ThrowIfNull(fragments);
        _fragments = (StyledString[])fragments.Clone();
    }

    /// <summary>
    /// Возвращает перечислитель текстовых фрагментов.
    /// </summary>
    /// <returns>Перечислитель фрагментов.</returns>
    public IEnumerator<StyledString> GetEnumerator()
    {
        return ((IEnumerable<StyledString>)_fragments).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
