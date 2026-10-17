using NBTerminal;

//Пример NBTerminal 0.2.0: две страницы, события клавиатуры и стилизованный вывод.
//F1 — счётчик, F2 — ввод текста, Escape — завершение приложения.

//Создаём страницы. Page(key) возвращает существующую страницу или создаёт новую.
Page counterPage = Terminal.Page("Counter");
Page textPage = Terminal.Page("Text");
Terminal.SelectPage("Counter");

//Данные приложения не принадлежат кадру и сохраняются между его обновлениями.
int counter = 0;
int resizeCount = 0;
string lastText = "Пока ничего не введено";

//Глобальные клавиши работают независимо от выбранной страницы.
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
            Terminal.Stop(); //Из обработчика только запрашиваем остановку, но не ждём её.
            break;
    }
};

//На странице счётчика обрабатываем стрелки.
counterPage.InputReadKey += key =>
{
    switch (key.Key)
    {
        case ConsoleKey.UpArrow:
            counter++;
            break;

        case ConsoleKey.DownArrow:
            counter--;
            break;
    }
};

//У каждой страницы свой буфер ввода. Enter передаёт завершённую строку.
textPage.InputReadLine += text =>
{
    lastText = text;
};

//Изменения размера обнаруживаются перед сборкой следующего кадра.
Terminal.OutputResized += (width, height) =>
{
    resizeCount++;
};

//Обработчик страницы заполняет свой участок кадра с помощью виртуального курсора Frame.
counterPage.Output += frame =>
{
    if (frame.Width < 55 || frame.Height < 15) return;

    frame.SetCursorPosition(0, 7);
    frame.WriteLine("СЧЁТЧИК".Bold().FgColor(100, 190, 255));
    frame.WriteLine();
    frame.WriteLine($"Значение: {counter}".FgColor(100, 255, 140));
    frame.WriteLine("Стрелка вверх — увеличить значение");
    frame.WriteLine("Стрелка вниз — уменьшить значение");
    frame.WriteLine();
    frame.Write("Оформление: ");
    frame.Write("RGB".Bold().FgColor(255, 100, 100));
    frame.Write(" и ");
    frame.WriteLine("палитра 256 цветов".FgColor((byte)46).Underline());
};

//На второй странице показываем текущую и последнюю отправленную строки.
textPage.Output += frame =>
{
    if (frame.Width < 55 || frame.Height < 15) return;

    frame.SetCursorPosition(0, 7);
    frame.WriteLine("ВВОД ТЕКСТА".Bold().FgColor(100, 190, 255));
    frame.WriteLine();
    frame.WriteLine("Введите текст и нажмите Enter.");
    frame.WriteLine($"Текущий ввод: {textPage.InputString}".FgColor(255, 220, 100));
    frame.WriteLine($"Последняя строка: {lastText}".FgColor(120, 230, 160));
    frame.WriteLine();
    frame.WriteLine("Незавершённый ввод сохранится при переключении страниц.");
};

//Глобальный Output вызывается ПОСЛЕ Output активной страницы.
//Возвращаем виртуальный курсор в начало и рисуем общую шапку поверх свободных строк.
Terminal.Output += frame =>
{
    frame.ResetCursor();

    if (frame.Width < 55 || frame.Height < 15)
    {
        frame.WriteLine("Увеличьте размер окна терминала.".FgColor(255, 110, 110));
        frame.WriteLine($"Текущий размер: {frame.Width} x {frame.Height}");
        return;
    }

    frame.WriteLine("NBTerminal 0.2.0 — пример использования".Bold().FgColor(80, 180, 255));
    frame.WriteLine();
    frame.WriteLine("F1 — счётчик | F2 — ввод текста | Esc — выход");
    frame.WriteLine($"Текущая страница: {Terminal.CurrentPage}");
    frame.WriteLine($"Размер: {frame.Width} x {frame.Height} | Изменений размера: {resizeCount}");
};

//Включаем обе подсистемы. Альтернативный экран используется по умолчанию.
Terminal.UseAlternateScreen = true;
Terminal.InputEnabled = true;
Terminal.OutputEnabled = true;

//Запускаем фоновый цикл и ожидаем его завершения из основного кода приложения.
Terminal.Start();
Terminal.Wait();

//После остановки рендер восстановил основной экран и кодировку вывода.
Console.WriteLine("NBTerminal остановлен.");
if (Terminal.MainException is { } exception)
    Console.WriteLine($"Ошибка главного цикла: {exception}");
