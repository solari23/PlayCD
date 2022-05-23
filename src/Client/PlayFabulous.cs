using Microsoft.Extensions.Options;
using PlayFabulous.Core.Config;

namespace PlayFabulous.Client
{
    public class PlayFabulous
    {
        private PlayFabConfig PlayFabConfig { get; }

        public PlayFabulous(IOptions<PlayFabConfig> configOptions)
        {
            this.PlayFabConfig = configOptions.Value;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Welcome to PlayFabulous: Creep Destroyer!");
            Console.WriteLine($"You will be playing PF title {this.PlayFabConfig.TitleId}");
            await Task.Yield();
        }
    }
}
