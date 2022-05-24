using PlayFab;
using PlayFab.ClientModels;
using PlayFabulous.Core;
using PlayFabulous.Models;

namespace PlayFabulous.Client;

public class CachingGameDataProvider : IGameDataProvider
{
    public CachingGameDataProvider(PlayerInfoService playerInfoService)
    {
        this.PlayerInfoService = playerInfoService;

        this.CharacterClasses = new Lazy<Task<IReadOnlyCollection<CharacterClass>>>(
            this.LoadCharacterClassesAsync);
        this.PlayerAllowedCharacterClasses = new Lazy<Task<IReadOnlyCollection<CharacterClass>>>(
            this.LoadPlayerAllowedCharacterClassesAsync);
    }

    private PlayerInfoService PlayerInfoService { get; }

    private Lazy<Task<IReadOnlyCollection<CharacterClass>>> CharacterClasses { get; }

    public async Task<IReadOnlyCollection<CharacterClass>> GetCharacterClassesAsync()
        => await this.CharacterClasses.Value;

    private Lazy<Task<IReadOnlyCollection<CharacterClass>>> PlayerAllowedCharacterClasses { get; }

    public async Task<IReadOnlyCollection<CharacterClass>> GetPlayerAllowedCharacterClassesAsync()
        => await this.PlayerAllowedCharacterClasses.Value;

    #region Factories

    private async Task<IReadOnlyCollection<CharacterClass>> LoadCharacterClassesAsync()
    {
        var getCatalogResult = await PlayFabClientAPI.GetCatalogItemsAsync(new GetCatalogItemsRequest
        {
            AuthenticationContext = await this.PlayerInfoService.GetPlayerAuthContextAsync(),
            CatalogVersion = CatalogNames.CharacterClasses,
        });

        if (getCatalogResult.Error is not null)
        {
            throw new Exception(
                $"Error while retrieving character classes: {getCatalogResult.Error.GenerateErrorReport()}");
        }

        return getCatalogResult.Result.Catalog
            .Where(c => c.CanBecomeCharacter)
            .Select(c =>
                new CharacterClass
                {
                    Id = c.ItemId,
                    DisplayName = c.DisplayName,
                    Description = c.Description,
                })
            .ToList();
    }

    private async Task<IReadOnlyCollection<CharacterClass>> LoadPlayerAllowedCharacterClassesAsync()
    {
        var inventoryResult = await PlayFabClientAPI.GetUserInventoryAsync(new GetUserInventoryRequest
        {
            AuthenticationContext = await this.PlayerInfoService.GetPlayerAuthContextAsync(),
        });

        if (inventoryResult.Error is not null)
        {
            throw new Exception(
                $"Error while retrieving character inventory: {inventoryResult.Error.GenerateErrorReport()}");
        }

        HashSet<string> allowedClassIds = inventoryResult.Result.Inventory
            .Where(c => c.CatalogVersion == CatalogNames.CharacterClasses)
            .Select(c => c.ItemId)
            .ToHashSet();

        return (await this.GetCharacterClassesAsync())
            .Where(c => allowedClassIds.Contains(c.Id))
            .ToList();
    }

    #endregion
}
