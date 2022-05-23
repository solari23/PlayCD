namespace PlayFabulous.Client;

public class IOService
{
    public Task<ConsoleKeyInfo> ReadKeyAsync() => Task.FromResult(Console.ReadKey());
    public Task<string> ReadLineAsync() => Task.FromResult(Console.ReadLine());
    public Task WriteLineAsync(string value)
    {
        Console.WriteLine(value);
        return Task.CompletedTask;
    }
}
