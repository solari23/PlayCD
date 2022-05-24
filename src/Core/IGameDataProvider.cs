using PlayFabulous.Models;

namespace PlayFabulous.Core;

public interface IGameDataProvider
{
    /// <summary>
    /// Gets all the <see cref="CharacterClass"/> types available in the game.
    /// </summary>
    /// <returns>The list of <see cref="CharacterClass"/> types.</returns>
    Task<IReadOnlyCollection<CharacterClass>> GetCharacterClassesAsync();

    /// <summary>
    /// Gets the list of <see cref="CharacterClass"/> types that the current player
    /// is allowed to use when creating characters.
    /// </summary>
    /// <returns>The list of <see cref="CharacterClass"/> types.</returns>
    Task<IReadOnlyCollection<CharacterClass>> GetPlayerAllowedCharacterClassesAsync();
}
