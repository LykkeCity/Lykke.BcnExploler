using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainGroupsByAsset
    {
        private ILookup<AssetViewModel, OffchainGroupViewModel> AssetChanneLookup { get; set; }
        private IEnumerable<OffchainGroupViewModel> BtcChannels { get; set; }

        public bool Exists(AssetViewModel asset)
        {
            return Get(asset).Any();
        }

        public IEnumerable<OffchainGroupViewModel> Get(AssetViewModel asset)
        {
            return AssetChanneLookup[asset];
        }

        public bool ExistsBtc()
        {
            return GetBtc().Any();
        }

        public IEnumerable<OffchainGroupViewModel> GetBtc()
        {
            return BtcChannels;
        }

        public static OffchainGroupsByAsset Create(IEnumerable<IGroup> groups, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return Create(groups.Select(p => OffchainGroupViewModel.Create(p, assetDictionary)));
        }

        public static OffchainGroupsByAsset Create(IEnumerable<OffchainGroupViewModel> channels)
        {

            return new OffchainGroupsByAsset
            {
                AssetChanneLookup = channels.Where(p => p.Asset.IsColored).ToLookup(p => p.Asset, AssetViewModel.AssetIdsComparer),
                BtcChannels = channels.Where(p => !p.Asset.IsColored)
            };
        }
    }
}
