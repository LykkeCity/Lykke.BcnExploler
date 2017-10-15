using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainChannelsByAsset
    {
        private ILookup<AssetViewModel, OffchainChannelViewModel> AssetChanneLookup { get; set; }
        private IEnumerable<OffchainChannelViewModel> BtcChannels { get; set; }

        public bool Exists(AssetViewModel asset)
        {
            return Get(asset).Any();
        }

        public IEnumerable<OffchainChannelViewModel> Get(AssetViewModel asset)
        {
            return AssetChanneLookup[asset];
        }

        public bool ExistsBtc()
        {
            return GetBtc().Any();
        }

        public IEnumerable<OffchainChannelViewModel> GetBtc()
        {
            return BtcChannels;
        }

        public static OffchainChannelsByAsset Create(IEnumerable<IChannel> channels, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return Create(channels.Select(p => OffchainChannelViewModel.Create(p, assetDictionary)));
        }

        public static OffchainChannelsByAsset Create(IEnumerable<OffchainChannelViewModel> channels)
        {

            return new OffchainChannelsByAsset
            {
                AssetChanneLookup = channels.Where(p => p.Asset.IsColored).ToLookup(p => p.Asset, AssetViewModel.AssetIdsComparer),
                BtcChannels = channels.Where(p => !p.Asset.IsColored)
            };
        }
    }
}
