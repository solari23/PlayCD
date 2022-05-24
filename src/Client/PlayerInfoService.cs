using PlayFab;
using PlayFab.ClientModels;
using PlayFabulous.Models;

namespace PlayFabulous.Client;

public class PlayerInfoService
{
    private LocalSaveStore LocalSaveStore { get; }

    public PlayerInfoService(
        IOService ioService,
        LocalSaveStore localSaveStore)
    {
        this.LocalSaveStore = localSaveStore;
    }

    private LoginResult cachedLoginCallResult = null;

    public void ClearLoggedInPlayerState() => this.cachedLoginCallResult = null;

    public async Task<PlayFabAuthenticationContext> GetPlayerAuthContextAsync()
        => (await this.GetCachedLoginResultAsync()).AuthenticationContext;

    private async Task<LoginResult> GetCachedLoginResultAsync()
    {
        if (this.cachedLoginCallResult is null)
        {
            LoginWithCustomIDRequest request;

            if (await this.LocalSaveStore.DoesSaveDataExistAsync())
            {
                var saveData = await this.LocalSaveStore.LoadAsync();

                request = new LoginWithCustomIDRequest
                {
                    CustomId = saveData.PlayerId,
                    CreateAccount = false,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetCharacterList = true,
                    },
                };
            }
            else
            {
                var saveData = new LocalSaveData
                {
                    PlayerId = Guid.NewGuid().ToString("N"),
                };
                await this.LocalSaveStore.SaveAsync(saveData);

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

            this.cachedLoginCallResult = loginResult.Result;
        }

        return this.cachedLoginCallResult;
    }
}
