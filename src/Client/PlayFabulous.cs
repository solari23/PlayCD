using Microsoft.Extensions.Options;
using PlayFab;
using PlayFab.ClientModels;
using PlayFabulous.Core.Config;
using PlayFabulous.Models;

namespace PlayFabulous.Client
{
    public class PlayFabulous
    {
        private PlayFabConfig PlayFabConfig { get; }
        private IOService IOService { get; }
        private LocalSaveStore LocalSaveStore { get; }

        public PlayFabulous(
            IOptions<PlayFabConfig> configOptions,
            IOService ioService,
            LocalSaveStore localSaveStore)
        {
            this.PlayFabConfig = configOptions.Value;
            this.IOService = ioService;
            this.LocalSaveStore = localSaveStore;
        }

        public async Task RunAsync()
        {
            this.InitializePlayFabSdk();

            await this.IOService.WriteLineAsync("Welcome to PlayFabulous: Creep Destroyer!");
            await this.IOService.WriteLineAsync($"You will be playing PF title {this.PlayFabConfig.TitleId}");

            var playerAuthContext = await this.LoginPlayerAsync();
            await this.IOService.WriteLineAsync($"Logged in as player with EntityId {playerAuthContext.EntityId}");
        }

        private void InitializePlayFabSdk()
        {
            PlayFabSettings.staticSettings.TitleId = this.PlayFabConfig.TitleId;
        }

        private async Task<PlayFabAuthenticationContext> LoginPlayerAsync()
        {
            LoginWithCustomIDRequest request;

            if (await this.LocalSaveStore.DoesSaveDataExistAsync())
            {
                await this.IOService.WriteLineAsync("Welcome back to the game!");
                var saveData = await this.LocalSaveStore.LoadAsync();

                request = new LoginWithCustomIDRequest
                {
                    CustomId = saveData.PlayerId,
                    CreateAccount = false,
                };
            }
            else
            {
                await this.IOService.WriteLineAsync("Welcome to the game! Let's get you set up..");

                var saveData = new LocalSaveData
                {
                    PlayerId = Guid.NewGuid().ToString("N"),
                };
                await this.LocalSaveStore.SaveAsync(saveData);
                await this.IOService.WriteLineAsync($"Saving a new account '{saveData.PlayerId}'");

                request = new LoginWithCustomIDRequest
                {
                    CustomId = saveData.PlayerId,
                    CreateAccount = true,
                };
            }

            var loginResult = await PlayFabClientAPI.LoginWithCustomIDAsync(request);
            if (loginResult.Error != null)
            {
                throw new Exception(
                    $"Login failed with error report: {loginResult.Error.GenerateErrorReport()}");
            }

            return loginResult.Result.AuthenticationContext;
        }
    }
}
