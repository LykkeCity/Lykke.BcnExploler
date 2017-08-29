using System;
using Lykke.Service.BcnExploler.Core;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Domain.Settings
{
    public static class BaseSettingsHelper
    {
        public static Network UsedNetwork(this BcnExplolerSettings baseSettings)
        {
            return Network.GetNetwork(baseSettings.Network);
        }
    }
}
