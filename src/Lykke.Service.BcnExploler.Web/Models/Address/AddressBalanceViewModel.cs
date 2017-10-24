using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Web.Models.Asset;
using Lykke.Service.BcnExploler.Web.Models.Offchain;

namespace Lykke.Service.BcnExploler.Web.Models.Address
{
    public class AddressBalanceViewModel
    {
        public string AddressId { get; set; }
        public double Balance { get; set; }
        public double UnconfirmedBalance => Balance + UnconfirmedBalanceDelta;
        public double UnconfirmedBalanceDelta { get; set; }
        public bool ShowUnconfirmedBalance => UnconfirmedBalanceDelta != 0;
        public double TotalConfirmedTransactions { get; set; }
        public bool TotalTransactionsCountCalculated { get; set; }
        public int TotalSpendedTransactions { get; set; }
        public bool TotalSpendedTransactionsCountCalculated { get; set; }

        public int TotalReceivedTransactions { get; set; }
        public bool TotalReceivedTransactionsCountCalculated { get; set; }

        public IEnumerable<ColoredBalance> Assets { get; set; }
        public AssetDictionary AssetDic { get; set; }

        public DateTime LastBlockDateTime { get; set; }
        public int LastBlockHeight { get; set; }

        public DateTime AtBlockDateTime { get; set; }
        public int AtBlockHeight { get; set; }

        public bool ShowNext => NextBlock <= LastBlockHeight;
        public bool ShowPrev => PrevBlock >= 0;
        public int PrevBlock => AtBlockHeight - 1;
        public int NextBlock => AtBlockHeight + 1;

        public bool IsOffchainHub { get; set; }

        public OffchainGroupsByAsset OffchainGroupsByAsset { get; set; }
        public static AddressBalanceViewModel Create(IAddressBalance balance, 
            IReadOnlyDictionary<string, IAssetDefinition> assetDictionary, 
            IBlockHeader lastBlock, 
            IBlockHeader atBlock,
            IEnumerable<IGroup> offchainGroups,
            bool isHub)
        {
            var onchainColoredBalances =
                (balance.ColoredBalances ?? Enumerable.Empty<IColoredBalance>()).Select(p =>
                    ColoredBalance.Create(p, assetDictionary)).ToList();

            var existedOnchainAssetsBalances = onchainColoredBalances.Select(p => p.AssetId).ToDictionary(p => p);

            //show balances for offchain assets with 0 onchain balance
            var missedOffchainColoredBalances = offchainGroups.Where(p => p.IsColored && !existedOnchainAssetsBalances.ContainsKey(p.AssetId))
                .Select(p => p.AssetId)
                .Distinct()
                .Select(assetId => ColoredBalance.CreateEmpty(assetId, assetDictionary));

            return new AddressBalanceViewModel
            {
                AddressId = balance.AddressId,
                TotalConfirmedTransactions = balance.TotalTransactions,
                Balance = balance.BtcBalance,
                Assets = onchainColoredBalances.Union(missedOffchainColoredBalances).ToList(),
                UnconfirmedBalanceDelta = balance.UnconfirmedBalanceDelta,
                AssetDic = AssetDictionary.Create(assetDictionary),
                LastBlockHeight = lastBlock.Height,
                LastBlockDateTime = lastBlock.Time,
                AtBlockHeight = (atBlock ?? lastBlock).Height,
                AtBlockDateTime = (atBlock ?? lastBlock).Time,
                OffchainGroupsByAsset = OffchainGroupsByAsset.Create(offchainGroups, assetDictionary),
                TotalTransactionsCountCalculated = balance.TotalTransactionsCountCalculated,
                TotalSpendedTransactions = balance.TotalSpendedTransactions,
                TotalSpendedTransactionsCountCalculated = balance.TotalSpendedTransactionsCountCalculated,
                TotalReceivedTransactions = balance.TotalReceivedTransactions,
                TotalReceivedTransactionsCountCalculated = balance.TotalReceivedTransactionsCountCalculated,
                IsOffchainHub = isHub
            };
        }
        
        public class ColoredBalance
        {
            public string AssetId { get; set; }

            public AssetViewModel Asset { get; set; }
            public double Quantity { get; set; }
            public double UnconfirmedQuantityDelta { get; set; }
            public bool ShowUnconfirmedBalance => UnconfirmedQuantityDelta != 0;
            public double UnconfirmedQuantity => Quantity + UnconfirmedQuantityDelta;

            public bool HasOnChainBalance => Quantity != 0 || UnconfirmedQuantity != 0;

            public static ColoredBalance Create(IColoredBalance coloredBalance, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
            {
                var asset = assetDictionary.GetValueOrDefault(coloredBalance.AssetId, null);

                var assetViewModel = asset != null
                    ? AssetViewModel.Create(asset)
                    : AssetViewModel.CreateNotFoundAsset(coloredBalance.AssetId);

                return new ColoredBalance
                {
                    AssetId = coloredBalance.AssetId,
                    Quantity = coloredBalance.Quantity,
                    UnconfirmedQuantityDelta = coloredBalance.UnconfirmedQuantityDelta,
                    Asset = assetViewModel,
                };
            }

            public static ColoredBalance CreateEmpty(string assetId, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
            {
                var asset = assetDictionary.GetValueOrDefault(assetId, null);

                var assetViewModel = asset != null
                    ? AssetViewModel.Create(asset)
                    : AssetViewModel.CreateNotFoundAsset(assetId);

                return new ColoredBalance
                {
                    AssetId = assetId,
                    Quantity = 0,
                    UnconfirmedQuantityDelta = 0,
                    Asset = assetViewModel,
                };
            }
        }
    }


}