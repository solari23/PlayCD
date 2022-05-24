using System.Text.Json;
using PlayFabulous.Models;

namespace PlayFabulous.Client;

public class LocalSaveStore
{
    private const string DefaultSaveFileName = "CreepDestroyer.save";

    private static string DefaultSaveDataDirectory => Path.Combine(
        Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? string.Empty,
        "PlayFabulous");

    public string SaveDataDirectory { get; }

    public string SaveDataFullFilePath { get; }

    public LocalSaveStore()
        : this(DefaultSaveDataDirectory)
    {
        // Empty.
    }

    public LocalSaveStore(string saveDataDirectory)
    {
        this.SaveDataDirectory = saveDataDirectory;
        this.SaveDataFullFilePath = Path.Combine(this.SaveDataDirectory, DefaultSaveFileName);
    }

    public Task<bool> DoesSaveDataExistAsync()
        => Task.FromResult(File.Exists(this.SaveDataFullFilePath));

    public async Task<LocalSaveData> LoadAsync()
    {
        var saveData = await File.ReadAllTextAsync(this.SaveDataFullFilePath);
        return JsonSerializer.Deserialize<LocalSaveData>(saveData);
    }

    public async Task SaveAsync(LocalSaveData saveData)
    {
        Directory.CreateDirectory(this.SaveDataDirectory);

        var saveJson = JsonSerializer.Serialize(saveData);
        await File.WriteAllTextAsync(this.SaveDataFullFilePath, saveJson);
    }
}
