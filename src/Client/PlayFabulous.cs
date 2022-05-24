using Microsoft.Extensions.Options;
using PlayFab;
using PlayFabulous.Core;
using PlayFabulous.Core.Config;
using PlayFabulous.Models;

namespace PlayFabulous.Client
{
    public class PlayFabulous
    {
        public PlayFabulous(
            IOptions<PlayFabConfig> configOptions,
            IOService ioService,
            PlayerInfoService playerInfoService,
            IGameDataProvider gameDataProvider)
        {
            this.PlayFabConfig = configOptions.Value;
            this.IOService = ioService;
            this.PlayerInfoService = playerInfoService;
            this.GameDataProvider = gameDataProvider;
        }

        private PlayFabConfig PlayFabConfig { get; }
        private IOService IOService { get; }
        private PlayerInfoService PlayerInfoService { get; }
        private IGameDataProvider GameDataProvider { get; }

        public async Task RunAsync()
        {
            await this.IOService.WriteLineAsync("Welcome to PlayFabulous: Creep Destroyer!");
            await this.IOService.WriteLineAsync($"You will be playing PF title {this.PlayFabConfig.TitleId}");

            var playerAuthContext = await this.PlayerInfoService.GetPlayerAuthContextAsync();
            await this.IOService.WriteLineAsync($"Logged in as player with EntityId {playerAuthContext.EntityId}");

            await this.DoStuffAsync();
        }

        // Random throwaway code during development.
        public async Task DoStuffAsync()
        {
            var classes = await this.GameDataProvider.GetCharacterClassesAsync();
            var allowedClasses = await this.GameDataProvider.GetPlayerAllowedCharacterClassesAsync();

            var targetClass = allowedClasses.Where(c => c.DisplayName == "Witch").First();
            var charInfo = await PlayFabClientAPI.GrantCharacterToUserAsync(new PlayFab.ClientModels.GrantCharacterToUserRequest
            {
                AuthenticationContext = await this.PlayerInfoService.GetPlayerAuthContextAsync(),
                CharacterName = "TheGuy",
                CatalogVersion = CatalogNames.CharacterClasses,
                ItemId = targetClass.Id,
            });
        }
    }
}
