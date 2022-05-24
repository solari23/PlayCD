using Microsoft.Extensions.Options;
using PlayFabulous.Core.Config;

namespace PlayFabulous.Client
{
    public class PlayFabulous
    {
        private PlayFabConfig PlayFabConfig { get; }
        private IOService IOService { get; }
        private PlayerInfoService PlayerInfoService { get; }

        public PlayFabulous(
            IOptions<PlayFabConfig> configOptions,
            IOService ioService,
            PlayerInfoService playerInfoService)
        {
            this.PlayFabConfig = configOptions.Value;
            this.IOService = ioService;
            this.PlayerInfoService = playerInfoService;
        }

        public async Task RunAsync()
        {
            await this.IOService.WriteLineAsync("Welcome to PlayFabulous: Creep Destroyer!");
            await this.IOService.WriteLineAsync($"You will be playing PF title {this.PlayFabConfig.TitleId}");

            var playerAuthContext = await this.PlayerInfoService.GetPlayerAuthContextAsync();
            await this.IOService.WriteLineAsync($"Logged in as player with EntityId {playerAuthContext.EntityId}");
        }
    }
}
