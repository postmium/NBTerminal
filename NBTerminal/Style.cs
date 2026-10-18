namespace NBTerminal;

/// <summary>
/// Хранит неизменяемое оформление: цвета текста и фона и набор текстовых атрибутов.
/// </summary>
/// <remarks>
/// <para>
/// Методы возвращают обновлённое значение. Исходный экземпляр не меняется; результат можно использовать в цепочке
/// вызовов или присвоить переменной.
/// </para>
/// <para>
/// Значение <c>default(Style)</c> не задаёт собственных цветов и не включает текстовые атрибуты. При выключенном цвете
/// рендер использует цвет терминала по умолчанию.
/// </para>
/// <para>
/// Индекс палитры хранится в поле R соответствующего цвета; в режиме RGB все три поля хранят компоненты. Настройка
/// одного цвета не меняет второй.
/// </para>
/// <para>
/// Стиль хранит намерение отрисовки, а не управляющие байты. Конкретные команды и приоритет быстрого мигания выбирает
/// рендер.
/// </para>
/// </remarks>
public readonly record struct Style
{
    /// <summary>
    /// Определяет свойства, явно установленные в данном стиле.
    /// </summary>
    internal StyleFlags Defined { get; private init; }

    //Цвет текста: R используется и для красной компоненты, и для индекса палитры.

    /// <summary>
    /// Красная компонента текста в RGB либо индекс его палитры.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte FgR { get; private init; }

    /// <summary>
    /// Зелёная компонента текста; при индексированной настройке обнуляется.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte FgG { get; private init; }

    /// <summary>
    /// Синяя компонента текста; при индексированной настройке обнуляется.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte FgB { get; private init; }

    //Цвет фона хранится отдельно от цвета текста.

    /// <summary>
    /// Красная компонента фона в RGB либо индекс его палитры.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte BgR { get; private init; }

    /// <summary>
    /// Зелёная компонента фона; при индексированной настройке обнуляется.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte BgG { get; private init; }

    /// <summary>
    /// Синяя компонента фона; при индексированной настройке обнуляется.
    /// </summary>
    /// <value>
    /// Байт от 0 до 255; интерпретация зависит от флагов соответствующего цвета.
    /// </value>
    internal byte BgB { get; private init; }

    /// <summary>
    /// Возвращает битовое представление режимов цветов и текстовых атрибутов.
    /// </summary>
    /// <value>
    /// Набор StyleFlags; нулевое значение соответствует оформлению по умолчанию.
    /// </value>
    internal StyleFlags Flags { get; private init; }

    //Низкоуровневые операции над битами. Они не меняют цветовые компоненты.

    /// <summary>
    /// Возвращает стиль с добавленными битами, не снимая остальные.
    /// </summary>
    /// <param name="flags">Биты, которые нужно включить.</param>
    /// <returns>
    /// Новое значение с результатом побитового ИЛИ.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Это низкоуровневая операция: связанные цветовые поля не изменяются и сочетания флагов дополнительно не
    /// проверяются.
    /// </para>
    /// </remarks>
    internal Style Add(StyleFlags flags)
    {
        return this with
        {
            Flags = Flags | flags
        };
    }

    /// <summary>
    /// Возвращает стиль со снятыми указанными битами.
    /// </summary>
    /// <param name="flags">Биты, которые нужно выключить.</param>
    /// <returns>
    /// Новое значение без указанных флагов; остальные биты и цветовые поля сохранены.
    /// </returns>
    internal Style Remove(StyleFlags flags)
    {
        return this with
        {
            Flags = Flags & ~flags
        };
    }

    /// <summary>
    /// Переключает указанные биты у возвращаемого стиля.
    /// </summary>
    /// <param name="flags">Биты, состояние которых нужно инвертировать.</param>
    /// <returns>
    /// Новое значение с результатом XOR (исключающего ИЛИ). Цветовые поля не меняются.
    /// </returns>
    internal Style Toggle(StyleFlags flags)
    {
        return this with
        {
            Flags = Flags ^ flags
        };
    }

    /// <summary>
    /// Включает или выключает группу флагов у возвращаемого стиля.
    /// </summary>
    /// <param name="flags">Биты для изменения.</param>
    /// <param name="enabled"><see langword="true"/> — добавить биты; <see langword="false"/> — снять.</param>
    /// <returns>
    /// Обновлённое значение без изменения исходного стиля.
    /// </returns>
    internal Style Set(StyleFlags flags, bool enabled)
    {
        return this with
        {
            Flags = enabled ? Flags | flags : Flags & ~flags,
            Defined = Defined | flags
        };
    }

    /// <summary>
    /// Проверяет, присутствуют ли все указанные биты.
    /// </summary>
    /// <param name="flags">Маска требуемых битов.</param>
    /// <returns>
    /// <see langword="true"/>, если присутствует каждый бит маски. Для пустой маски результат также равен
    /// <see langword="true"/>.
    /// </returns>
    internal bool Has(StyleFlags flags)
    {
        return (Flags & flags) == flags;
    }

    /// <summary>
    /// Проверяет, присутствует ли хотя бы один указанный бит.
    /// </summary>
    /// <param name="flags">Маска проверяемых битов.</param>
    /// <returns>
    /// <see langword="true"/>, если найден общий бит. Для пустой маски результат равен <see langword="false"/>.
    /// </returns>
    internal bool HasAny(StyleFlags flags)
    {
        return (Flags & flags) != 0;
    }

    //Сброс оформления: полный либо только текстовых атрибутов.

    /// <summary>
    /// Сбрасывает цвета текста и фона, а также все текстовые атрибуты.
    /// </summary>
    /// <returns>
    /// Новое значение стиля без включённых цветов и текстовых атрибутов.
    /// Все свойства помечаются как явно заданные для переопределения унаследованного оформления.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Метод возвращает новый стиль, не изменяя исходное значение.
    /// </para>
    /// <para>
    /// В отличие от <c>default(Style)</c>, возвращаемое значение содержит маску
    /// <c>Defined</c>, которая позволяет отменять оформление родительского
    /// <see cref="StyledText"/> при наследовании стилей.
    /// </para>
    /// </remarks>
    public Style ClearStyles()
    {
        return new Style
        {
            Defined = StyleFlags.FgEnabled | StyleFlags.BgEnabled | StyleFlags.TextMask
        };
    }

    /// <summary>
    /// Снимает все текстовые атрибуты, оставляя цвета без изменений.
    /// </summary>
    /// <returns>
    /// Новое значение с сохранёнными цветами текста и фона и без флагов TextMask.
    /// </returns>
    public Style ClearTextStyles()
    {
        return this with
        {
            Flags = Flags & ~StyleFlags.TextMask,
            Defined = Defined | StyleFlags.TextMask
        };
    }

    /// <summary>
    /// Объединяет родительский и дочерний стили.
    /// Явно заданные свойства дочернего стиля имеют приоритет.
    /// </summary>
    internal static Style Merge(Style parent, Style child)
    {
        StyleFlags textDefined = child.Defined & StyleFlags.TextMask;
        StyleFlags flags = (parent.Flags & ~textDefined) | (child.Flags & textDefined);
        StyleFlags defined = parent.Defined | child.Defined;

        byte fgR = parent.FgR;
        byte fgG = parent.FgG;
        byte fgB = parent.FgB;

        byte bgR = parent.BgR;
        byte bgG = parent.BgG;
        byte bgB = parent.BgB;

        if ((child.Defined & StyleFlags.FgEnabled) != 0)
        {
            flags = (flags & ~StyleFlags.FgMask) | (child.Flags & StyleFlags.FgMask);
            fgR = child.FgR;
            fgG = child.FgG;
            fgB = child.FgB;
        }

        if ((child.Defined & StyleFlags.BgEnabled) != 0)
        {
            flags = (flags & ~StyleFlags.BgMask) | (child.Flags & StyleFlags.BgMask);
            bgR = child.BgR;
            bgG = child.BgG;
            bgB = child.BgB;
        }

        return new Style
        {
            FgR = fgR,
            FgG = fgG,
            FgB = fgB,
            BgR = bgR,
            BgG = bgG,
            BgB = bgB,
            Flags = flags,
            Defined = defined
        };
    }

    /// <summary>
    /// Возвращает стиль без метаданных наследования для записи в кадр.
    /// </summary>
    internal Style Resolved()
    {
        return this with { Defined = 0 };
    }

    //Настройка текстовых атрибутов: каждый метод возвращает новый Style.

    /// <summary>
    /// Включает или выключает жирное начертание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить жирное начертание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Bold(bool enabled = true) => Set(StyleFlags.Bold, enabled);

    /// <summary>
    /// Включает или выключает тусклое отображение у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить тусклое отображение; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Dim(bool enabled = true) => Set(StyleFlags.Dim, enabled);

    /// <summary>
    /// Включает или выключает курсив у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled"><see langword="true"/> — включить курсив; <see langword="false"/> — выключить.</param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Italic(bool enabled = true) => Set(StyleFlags.Italic, enabled);

    /// <summary>
    /// Включает или выключает подчёркивание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить подчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Underline(bool enabled = true) => Set(StyleFlags.Underline, enabled);

    /// <summary>
    /// Включает или выключает зачёркивание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить зачёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Strikethrough(bool enabled = true) => Set(StyleFlags.Strikethrough, enabled);

    /// <summary>
    /// Включает или выключает надчёркивание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить надчёркивание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Overline(bool enabled = true) => Set(StyleFlags.Overline, enabled);

    /// <summary>
    /// Включает или выключает обычное мигание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить обычное мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Blink(bool enabled = true) => Set(StyleFlags.Blink, enabled);

    /// <summary>
    /// Включает или выключает быстрое мигание у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить быстрое мигание; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style RapidBlink(bool enabled = true) => Set(StyleFlags.RapidBlink, enabled);

    /// <summary>
    /// Включает или выключает инверсию цветов текста и фона у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить инверсию цветов текста и фона; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Invert(bool enabled = true) => Set(StyleFlags.Invert, enabled);

    /// <summary>
    /// Включает или выключает скрытие текста у возвращаемого стиля.
    /// </summary>
    /// <param name="enabled">
    /// <see langword="true"/> — включить скрытие текста; <see langword="false"/> — выключить.
    /// </param>
    /// <returns>
    /// Новое значение с изменённым атрибутом; другие флаги и цвета сохраняются.
    /// </returns>
    public Style Conceal(bool enabled = true) => Set(StyleFlags.Conceal, enabled);

    //Цвет текста: сброс или установка индексированной палитры либо RGB.

    /// <summary>
    /// Выбирает цвет текста по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <returns>
    /// Новое значение стиля без включённого собственного цвета текста.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style FgDefault()
    {
        return this with
        {
            //Очищаем неиспользуемые компоненты и оба бита режима цвета текста.
            FgR = 0,
            FgG = 0,
            FgB = 0,
            Flags = Flags & ~StyleFlags.FgMask,
            Defined = Defined | StyleFlags.FgEnabled
        };
    }

    /// <summary>
    /// Задаёт цвет текста индексом палитры терминала.
    /// </summary>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новое значение стиля с индексированным цветом текста.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style FgColor(byte index)
    {
        return this with
        {
            //Индекс записывается в R; RGB-признак снимается, цвет остаётся включённым.
            FgR = index,
            FgG = 0,
            FgB = 0,
            Flags = (Flags & ~StyleFlags.FgMask) | StyleFlags.FgEnabled,
            Defined = Defined | StyleFlags.FgEnabled
        };
    }

    /// <summary>
    /// Задаёт цвет текста тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новое значение стиля с цветом текста в режиме RGB.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style FgColor(byte r, byte g, byte b)
    {
        return this with
        {
            //Включаем собственный цвет текста и его RGB-режим.
            FgR = r,
            FgG = g,
            FgB = b,
            Flags = (Flags & ~StyleFlags.FgMask) | StyleFlags.FgEnabled | StyleFlags.FgTrueColor,
            Defined = Defined | StyleFlags.FgEnabled
        };
    }

    //Цвет фона: те же режимы, но отдельные компоненты и флаги.

    /// <summary>
    /// Выбирает цвет фона по умолчанию вместо явно заданного цвета.
    /// </summary>
    /// <returns>
    /// Новое значение стиля без включённого собственного цвета фона.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style BgDefault()
    {
        return this with
        {
            //Сбрасываем собственный цвет фона, не затрагивая текст и текстовые атрибуты.
            BgR = 0,
            BgG = 0,
            BgB = 0,
            Flags = Flags & ~StyleFlags.BgMask,
            Defined = Defined | StyleFlags.BgEnabled
        };
    }

    /// <summary>
    /// Задаёт цвет фона индексом палитры терминала.
    /// </summary>
    /// <param name="index">Индекс цвета от 0 до 255. В структуре стиля хранится в соответствующем поле R.</param>
    /// <returns>
    /// Новое значение стиля с индексированным цветом фона.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style BgColor(byte index)
    {
        return this with
        {
            //Индекс фона хранится в BgR, остальные компоненты обнуляются.
            BgR = index,
            BgG = 0,
            BgB = 0,
            Flags = (Flags & ~StyleFlags.BgMask) | StyleFlags.BgEnabled,
            Defined = Defined | StyleFlags.BgEnabled
        };
    }

    /// <summary>
    /// Задаёт цвет фона тремя компонентами RGB (красной, зелёной и синей).
    /// </summary>
    /// <param name="r">Красная компонента от 0 до 255.</param>
    /// <param name="g">Зелёная компонента от 0 до 255.</param>
    /// <param name="b">Синяя компонента от 0 до 255.</param>
    /// <returns>
    /// Новое значение стиля с цветом фона в режиме RGB.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Остальные цвета и текстовые атрибуты сохраняются; исходное значение не изменяется.
    /// </para>
    /// </remarks>
    public Style BgColor(byte r, byte g, byte b)
    {
        return this with
        {
            //Включаем собственный цвет фона и его RGB-режим.
            BgR = r,
            BgG = g,
            BgB = b,
            Flags = (Flags & ~StyleFlags.BgMask) | StyleFlags.BgEnabled | StyleFlags.BgTrueColor,
            Defined = Defined | StyleFlags.BgEnabled
        };
    }
}