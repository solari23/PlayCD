using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlayFabulous.Core.Config;

namespace PlayFabulous.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("Config.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Add services to the DI container.
                services.AddTransient<PlayFabulous>();

                // Bind configuration objects.
                var configRoot = context.Configuration;
                services.Configure<PlayFabConfig>(configRoot.GetSection(nameof(PlayFabConfig)));
            })
            .Build();

        var game = host.Services.GetRequiredService<PlayFabulous>();
        await game.RunAsync();
    }
}
