namespace NBTerminal;

/// <summary>
/// Хранит собственный ввод и события одной страницы консольного интерфейса.
/// </summary>
/// <remarks>
/// <para>
/// Каждая страница независимо накапливает строку, последнюю клавишу и последний обычный символ. Переключение страницы
/// не сбрасывает эти данные.
/// </para>
/// <para>
/// Сама страница не хранит экран и не запускает цикл: Terminal передаёт ей ввод и общий кадр в соответствующих
/// событиях. Экземпляры не предоставляют гарантии потокобезопасности.
/// </para>
/// </remarks>
public class Page
{

    //Локальный ввод страницы сохраняется при её переключении и не заменяет глобальный ввод.
    //Последняя клавиша, дошедшая до страничного обработчика.
    private ConsoleKeyInfo? _inputKeyInfo;

    //Последний добавленный обычный символ.
    private char? _inputChar;

    //Незавершённая строка именно этой страницы.
    private volatile string _inputString = string.Empty;

    /// <summary>
    /// Возвращает последнюю клавишу, переданную именно этой странице.
    /// </summary>
    /// <value>
    /// ConsoleKeyInfo или <see langword="null"/> до первой передачи.
    /// </value>
    /// <remarks>
    /// <para>
    /// Сохраняется между переключениями страниц и запусками терминала. Если глобальный обработчик прервал обработку
    /// клавиши исключением, эта страница её не получит.
    /// </para>
    /// </remarks>
    public ConsoleKeyInfo? InputKeyInfo => _inputKeyInfo;

    /// <summary>
    /// Возвращает последний обычный символ, принятый страницей.
    /// </summary>
    /// <value>
    /// Символ или <see langword="null"/> до первого приёма.
    /// </value>
    /// <remarks>
    /// <para>
    /// Enter и Backspace не очищают это свойство.
    /// </para>
    /// </remarks>
    public char? InputChar => _inputChar;

    /// <summary>
    /// Возвращает незавершённую строку ввода этой страницы.
    /// </summary>
    /// <value>
    /// Символы, поступившие странице после её последнего Enter.
    /// </value>
    /// <remarks>
    /// <para>
    /// Не обязана совпадать с Terminal.InputString. При переключении страницы сохраняется; при Enter очищается до
    /// события InputReadLine; Backspace удаляет последний char.
    /// </para>
    /// </remarks>
    public string InputString => _inputString;

    //События страницы вызываются терминалом; сама страница не запускает обработку.
    /// <summary>
    /// Возникает при передаче клавиши странице после глобального события клавиши.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Аргумент содержит сведения о клавише. InputKeyInfo страницы уже обновлён, а обработка Enter, Backspace или
    /// добавление символа ещё не выполнены. Исключение передаётся в Terminal.InputError.
    /// </para>
    /// </remarks>
    public event Action<ConsoleKeyInfo>? InputReadKey;

    /// <summary>
    /// Возникает после добавления обычного символа во ввод страницы.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Аргумент — добавленный char. InputChar и InputString уже обновлены. Исключение прерывает остальных подписчиков
    /// этого вызова и передаётся в Terminal.InputError.
    /// </para>
    /// </remarks>
    public event Action<char>? InputRead;

    /// <summary>
    /// Возникает при завершении строки страницы клавишей Enter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Аргумент — строка до очистки InputString, без завершающей клавиши. Глобальное событие строки вызывается раньше;
    /// при его исключении выполнение до этого события не дойдёт.
    /// </para>
    /// </remarks>
    public event Action<string>? InputReadLine;

    /// <summary>
    /// Возникает для заполнения кадра, если страница выбрана для текущего вывода.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Аргумент — очищенный или заново созданный Frame. После страничных подписчиков он передаётся глобальным
    /// обработчикам Terminal.Output и затем в рендер.
    /// </para>
    /// <para>
    /// Ссылка на кадр переиспользуется терминалом. Его можно менять только во время синхронной сборки; исключение
    /// подписчика направляется в Terminal.OutputError, а кадр не отображается.
    /// </para>
    /// </remarks>
    public event Action<Frame>? Output;

    /// <summary>
    /// Уведомляет активную страницу об обнаруженном изменении размеров вывода.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Первый аргумент — ширина, второй — высота. Вызывается до глобального Terminal.OutputResized и сборки кадра.
    /// Первое измерение после запуска рендера не уведомляется, неактивные страницы не получают пропущенные уведомления
    /// при переключении.
    /// </para>
    /// </remarks>
    public event Action<int, int>? OutputResized;

    /// <summary>
    /// Сохраняет последнюю клавишу страницы и вызывает её событие.
    /// </summary>
    /// <param name="obj">Клавиша, которую Terminal передаёт этой странице.</param>
    internal void OnInputReadKey(ConsoleKeyInfo obj)
    {
        //Состояние обновляется раньше события, чтобы подписчик видел новое значение.
        _inputKeyInfo = obj;
        InputReadKey?.Invoke(obj);
    }

    /// <summary>
    /// Добавляет символ во ввод страницы и вызывает InputRead.
    /// </summary>
    /// <param name="obj">Уже отобранный терминалом обычный символ.</param>
    /// <remarks>
    /// <para>
    /// Изменения InputChar и InputString происходят до события и не откатываются при исключении подписчика.
    /// </para>
    /// </remarks>
    internal void OnInputRead(char obj)
    {
        //Добавление выполняется до события и не откатывается при ошибке подписчика.
        _inputChar = obj;
        _inputString += obj;

        InputRead?.Invoke(obj);
    }

    /// <summary>
    /// Передаёт накопленную строку подписчикам и очищает ввод страницы.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сначала сохраняет строку в локальной переменной, затем очищает поле и только после этого вызывает InputReadLine.
    /// </para>
    /// </remarks>
    internal void OnInputReadLine()
    {
        //Строка для события сохраняется отдельно, а накопитель очищается до вызова обработчиков.
        string line = _inputString;
        _inputString = string.Empty;

        InputReadLine?.Invoke(line);
    }

    /// <summary>
    /// Удаляет последний char из незавершённой строки страницы, если она не пуста.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Не меняет последние клавишу и символ и не вызывает отдельного события удаления.
    /// </para>
    /// </remarks>
    internal void OnBackspace()
    {
        //Удаляем последнюю кодовую единицу char, не проверяя составные символы.
        if (_inputString.Length > 0) _inputString = _inputString[..^1];
    }

    /// <summary>
    /// Передаёт общий кадр обработчикам вывода страницы.
    /// </summary>
    /// <param name="frame">Кадр, который подписчики заполняют синхронно.</param>
    internal void OnOutput(Frame frame)
    {
        Output?.Invoke(frame);
    }

    /// <summary>
    /// Вызывает событие новых размеров этой страницы.
    /// </summary>
    /// <param name="width">Новая ширина в ячейках.</param>
    /// <param name="height">Новая высота в ячейках.</param>
    internal void OnOutputResized(int width, int height)
    {
        OutputResized?.Invoke(width, height);
    }
}