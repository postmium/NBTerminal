namespace NBTerminal;

/// <summary>
/// Хранит текст и единое оформление для всех его символов.
/// </summary>
/// <remarks>
/// <para>
/// Это неизменяемая структура: методы настройки возвращают новое значение, а не меняют существующее. Для сохранения
/// изменения нужно использовать возвращённый результат.
/// </para>
/// <para>
/// Сама структура не содержит управляющих команд и ничего не рисует. При записи в <see cref="Frame"/> текст и стиль
/// переносятся в ячейки.
/// </para>
/// <para>
/// Конструкторы запрещают <see langword="null"/> в тексте. Однако <c>default(StyledString)</c> обходит конструкторы и
/// содержит пустую ссылку: <see cref="ToString"/> вернёт пустую строку, а методы изменения оформления, создающие новое
/// значение через конструктор, выбросят исключение.
/// </para>
/// </remarks>
public readonly record struct StyledString
{
    /// <summary>
    /// Возвращает текст, связанный с оформлением.
    /// </summary>
    /// <value>
    /// Исходная строка; у <c>default(StyledString)</c> фактически равна <see langword="null"/>.
    /// </value>
    public string Text { get; }

    /// <summary>
    /// Возвращает оформление, которое применяется ко всему тексту.
    /// </summary>
    /// <value>
    /// Цвета и текстовые атрибуты; само свойство не применяет их к терминалу.
    /// </value>
    public Style Style { get; }

    /// <summary>
    /// Создаёт стилизованную строку с оформлением по умолчанию.
    /// </summary>
    /// <param name="text">Текст строки.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public StyledString(string text) : this(text, default)
    {

    }

    /// <summary>
    /// Связывает текст с заданным оформлением.
    /// </summary>
    /// <param name="text">Текст строки.</param>
    /// <param name="style">Общий стиль всех символов.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> равен <see langword="null"/>.</exception>
    public StyledString(string text, Style style)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Style = style;
    }

    /// <summary>
    /// Заменяет всё оформление, сохраняя текст.
    /// </summary>
    /// <param name="style">Новый стиль; прежний стиль с ним не объединяется.</param>
    /// <returns>
    /// Новое значение с тем же текстом и переданным стилем.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString WithStyle(Style style)
    {
        return new(Text, style);
    }

    /// <summary>
    /// Сбрасывает цвета и все текстовые атрибуты.
    /// </summary>
    /// <returns>
    /// Новое значение с тем же текстом и оформлением по умолчанию.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString ClearStyles()
    {
        return WithStyle(Style.ClearStyles());
    }

    /// <summary>
    /// Сбрасывает только текстовые атрибуты, сохраняя цвета текста и фона.
    /// </summary>
    /// <returns>
    /// Новое значение с прежним текстом и цветами, но без текстовых атрибутов.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString ClearTextStyles()
    {
        return WithStyle(Style.ClearTextStyles());
    }

    //Изменение отдельных атрибутов не затрагивает остальные части стиля.

    /// <summary>
    /// Включает или выключает жирное начертание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить жирное начертание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Bold(bool enabled = true) => WithStyle(Style.Bold(enabled));

    /// <summary>
    /// Включает или выключает тусклое отображение у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить тусклое отображение; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Dim(bool enabled = true) => WithStyle(Style.Dim(enabled));

    /// <summary>
    /// Включает или выключает курсив у возвращаемой строки.
    /// </summary>
    /// <param name="enabled"><see langword="true"/> — включить курсив; <see langword="false"/> — выключить.</param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Italic(bool enabled = true) => WithStyle(Style.Italic(enabled));

    /// <summary>
    /// Включает или выключает подчёркивание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить подчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Underline(bool enabled = true) => WithStyle(Style.Underline(enabled));

    /// <summary>
    /// Включает или выключает зачёркивание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить зачёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Strikethrough(bool enabled = true) => WithStyle(Style.Strikethrough(enabled));

    /// <summary>
    /// Включает или выключает надчёркивание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить надчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Overline(bool enabled = true) => WithStyle(Style.Overline(enabled));

    /// <summary>
    /// Включает или выключает обычное мигание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить обычное мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Blink(bool enabled = true) => WithStyle(Style.Blink(enabled));

    /// <summary>
    /// Включает или выключает быстрое мигание у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить быстрое мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString RapidBlink(bool enabled = true) => WithStyle(Style.RapidBlink(enabled));

    /// <summary>
    /// Включает или выключает инверсию цветов текста и фона у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить инверсию цветов текста и фона; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Invert(bool enabled = true) => WithStyle(Style.Invert(enabled));

    /// <summary>
    /// Включает или выключает скрытие текста у возвращаемой строки.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить скрытие текста; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с тем же текстом и изменённым атрибутом. Остальное оформление сохраняется.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Свойство <see cref="Text"/> текущего значения равно <see langword="null"/>, например у
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString Conceal(bool enabled = true) => WithStyle(Style.Conceal(enabled));

    //Настройка цвета текста.

    /// <summary>
    /// Выбирает цвет текста по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <returns>
    /// Новая стилизованная строка без включённого собственного цвета текста.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString FgDefault() => WithStyle(Style.FgDefault());

    /// <summary>
    /// Задаёт цвет текста индексом палитры терминала.
    /// </summary>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новая стилизованная строка с индексированным цветом текста.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString FgColor(byte index) => WithStyle(Style.FgColor(index));

    /// <summary>
    /// Задаёт цвет текста тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новая стилизованная строка с цветом текста в режиме RGB.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString FgColor(byte r, byte g, byte b) => WithStyle(Style.FgColor(r, g, b));

    //Настройка цвета фона.

    /// <summary>
    /// Выбирает цвет фона по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <returns>
    /// Новая стилизованная строка без включённого собственного цвета фона.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString BgDefault() => WithStyle(Style.BgDefault());

    /// <summary>
    /// Задаёт цвет фона индексом палитры терминала.
    /// </summary>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новая стилизованная строка с индексированным цветом фона.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString BgColor(byte index) => WithStyle(Style.BgColor(index));

    /// <summary>
    /// Задаёт цвет фона тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новая стилизованная строка с цветом фона в режиме RGB.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Текущее значение содержит <see langword="null"/> в свойстве <see cref="Text"/>, например после
    /// <c>default(StyledString)</c>.
    /// </exception>
    public StyledString BgColor(byte r, byte g, byte b) => WithStyle(Style.BgColor(r, g, b));

    /// <summary>
    /// Возвращает только текст без команд оформления.
    /// </summary>
    /// <returns>
    /// Исходный текст или пустая строка, если текст равен <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Цвета и текстовые атрибуты в результат не включаются.
    /// </para>
    /// </remarks>
    public override string ToString()
    {
        return Text ?? string.Empty;
    }
}