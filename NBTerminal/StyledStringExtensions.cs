namespace NBTerminal;

/// <summary>
/// Создаёт стилизованные строки из обычного текста с помощью методов расширения.
/// </summary>
/// <remarks>
/// <para>
/// Методы не изменяют исходную строку и ничего не выводят в консоль. Результат содержит тот же текст и выбранное
/// оформление.
/// </para>
/// <para>
/// Оформление можно дополнять цепочкой вызовов. Для записи результата в кадр используется
/// <see cref="Frame.Write(StyledString)"/>.
/// </para>
/// </remarks>
public static class StyledStringExtensions
{
    /// <summary>
    /// Оборачивает текст в стилизованную строку с оформлением по умолчанию.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <returns>
    /// Значение <see cref="StyledString"/> без явно заданных цветов и текстовых атрибутов.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Styled(this string text) => new(text);

    //Текстовые атрибуты: методы создают значение, которое затем можно дополнить другими вызовами.

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает жирное начертание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить жирное начертание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Bold(this string text, bool enabled = true) => new StyledString(text).Bold(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает тусклое отображение.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить тусклое отображение; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Dim(this string text, bool enabled = true) => new StyledString(text).Dim(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает курсив.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled"><see langword="true"/> — включить курсив; <see langword="false"/> — выключить.</param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Italic(this string text, bool enabled = true) => new StyledString(text).Italic(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает подчёркивание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить подчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Underline(this string text, bool enabled = true) => new StyledString(text).Underline(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает зачёркивание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить зачёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Strikethrough(this string text, bool enabled = true) => new StyledString(text).Strikethrough(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает надчёркивание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить надчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Overline(this string text, bool enabled = true) => new StyledString(text).Overline(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает обычное мигание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить обычное мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Blink(this string text, bool enabled = true) => new StyledString(text).Blink(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает быстрое мигание.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить быстрое мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString RapidBlink(this string text, bool enabled = true) => new StyledString(text).RapidBlink(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает инверсию цветов текста и фона.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить инверсию цветов текста и фона; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Invert(this string text, bool enabled = true) => new StyledString(text).Invert(enabled);

    /// <summary>
    /// Создаёт стилизованную строку и включает или выключает скрытие текста.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="enabled">
    /// <see langword="true"/> — включить скрытие текста; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новая стилизованная строка с указанным состоянием атрибута.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString Conceal(this string text, bool enabled = true) => new StyledString(text).Conceal(enabled);

    //Цвет текста: стандартный, индексированный или RGB (три компоненты цвета).

    /// <summary>
    /// Выбирает цвет текста по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <returns>
    /// Новая стилизованная строка без включённого собственного цвета текста.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString FgDefault(this string text) => new StyledString(text).FgDefault();

    /// <summary>
    /// Задаёт цвет текста индексом палитры терминала.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новая стилизованная строка с индексированным цветом текста.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString FgColor(this string text, byte index) => new StyledString(text).FgColor(index);

    /// <summary>
    /// Задаёт цвет текста тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новая стилизованная строка с цветом текста в режиме RGB.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString FgColor(this string text, byte r, byte g, byte b) => new StyledString(text).FgColor(r, g, b);

    //Цвет фона задаётся независимо от цвета текста.

    /// <summary>
    /// Выбирает цвет фона по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <returns>
    /// Новая стилизованная строка без включённого собственного цвета фона.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString BgDefault(this string text) => new StyledString(text).BgDefault();

    /// <summary>
    /// Задаёт цвет фона индексом палитры терминала.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новая стилизованная строка с индексированным цветом фона.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString BgColor(this string text, byte index) => new StyledString(text).BgColor(index);

    /// <summary>
    /// Задаёт цвет фона тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новая стилизованная строка с цветом фона в режиме RGB.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString BgColor(this string text, byte r, byte g, byte b) => new StyledString(text).BgColor(r, g, b);

    /// <summary>
    /// Создаёт стилизованную строку с полностью заданным оформлением.
    /// </summary>
    /// <param name="text">Исходный текст.</param>
    /// <param name="style">Оформление, которое нужно связать с текстом.</param>
    /// <returns>
    /// Новая стилизованная строка с переданным стилем.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public static StyledString WithStyle(this string text, Style style) => new(text, style);
}