using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PlayFab;
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
                services.AddSingleton<IOService>();
                services.AddSingleton<LocalSaveStore>();
                services.AddSingleton<PlayerInfoService>();

                // Bind configuration objects.
                var configRoot = context.Configuration;
                services.Configure<PlayFabConfig>(configRoot.GetSection(nameof(PlayFabConfig)));
            })
            .Build();

        var playFabConfig = host.Services.GetRequiredService<IOptions<PlayFabConfig>>().Value;
        InitializePlayFabSdk(playFabConfig);

        var game = host.Services.GetRequiredService<PlayFabulous>();
        await game.RunAsync();
    }

    private static void InitializePlayFabSdk(PlayFabConfig config)
    {
        PlayFabSettings.staticSettings.TitleId = config.TitleId;
    }
}
