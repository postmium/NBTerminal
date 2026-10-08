using NBTerminal;

Terminal.InputReadKey += key =>
{
    Console.WriteLine(key.Key);
};

Terminal.StartInput();

await Task.Delay(5000);

await Terminal.StopInputAsync();