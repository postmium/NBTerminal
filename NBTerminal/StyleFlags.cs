namespace NBTerminal;

/// <summary>
/// Определяет биты режима цветов и текстовых атрибутов внутри Style.
/// </summary>
/// <remarks>
/// <para>
/// Fg относится к тексту, Bg — к фону. Enabled разрешает явно заданный цвет; TrueColor выбирает RGB вместо
/// индексированной палитры, только если соответствующий Enabled включён.
/// </para>
/// <para>
/// Значения FgMask, BgMask и TextMask объединяют существующие биты и не добавляют новых признаков. Флаги сами по себе
/// ничего не выводят.
/// </para>
/// </remarks>
[Flags]
internal enum StyleFlags : ushort
{
    /// <summary>
    /// Не задаёт ни одного флага: цвета и текстовые атрибуты остаются стандартными.
    /// </summary>
    None = 0,

    /// <summary>
    /// Разрешает явно заданный цвет текста.
    /// </summary>
    FgEnabled = 1 << 0,

    /// <summary>
    /// Выбирает RGB для включённого цвета текста; без этого бита используется индекс палитры.
    /// </summary>
    FgTrueColor = 1 << 1,

    /// <summary>
    /// Разрешает явно заданный цвет фона.
    /// </summary>
    BgEnabled = 1 << 2,

    /// <summary>
    /// Выбирает RGB для включённого цвета фона; без этого бита используется индекс палитры.
    /// </summary>
    BgTrueColor = 1 << 3,

    /// <summary>
    /// Запрашивает жирное начертание.
    /// </summary>
    Bold = 1 << 4,

    /// <summary>
    /// Запрашивает тусклое отображение.
    /// </summary>
    Dim = 1 << 5,

    /// <summary>
    /// Запрашивает курсив.
    /// </summary>
    Italic = 1 << 6,

    /// <summary>
    /// Запрашивает подчёркивание.
    /// </summary>
    Underline = 1 << 7,

    /// <summary>
    /// Запрашивает зачёркивание.
    /// </summary>
    Strikethrough = 1 << 8,

    /// <summary>
    /// Запрашивает надчёркивание.
    /// </summary>
    Overline = 1 << 9,

    /// <summary>
    /// Запрашивает обычное мигание.
    /// </summary>
    Blink = 1 << 10,

    /// <summary>
    /// Запрашивает быстрое мигание; в текущем рендере имеет приоритет над обычным.
    /// </summary>
    RapidBlink = 1 << 11,

    /// <summary>
    /// Запрашивает инверсию цветов текста и фона.
    /// </summary>
    Invert = 1 << 12,

    /// <summary>
    /// Запрашивает скрытие текста без удаления символа из кадра.
    /// </summary>
    Conceal = 1 << 13,

    /// <summary>
    /// Объединяет биты включения и режима цвета текста.
    /// </summary>
    FgMask = FgEnabled | FgTrueColor,

    /// <summary>
    /// Объединяет биты включения и режима цвета фона.
    /// </summary>
    BgMask = BgEnabled | BgTrueColor,

    /// <summary>
    /// Объединяет все текстовые атрибуты, не затрагивая режимы цветов.
    /// </summary>
    TextMask = Bold | Dim | Italic | Underline | Strikethrough | Overline | Blink | RapidBlink | Invert | Conceal
}