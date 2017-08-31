using Lykke.Service.BcnExploler.Core.Settings;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Settings
{
    public static class BaseSettingsHelper
    {
        public static Network UsedNetwork(this BcnExplolerSettings baseSettings)
        {
            return Network.GetNetwork(baseSettings.Network);
        }
    }
}
