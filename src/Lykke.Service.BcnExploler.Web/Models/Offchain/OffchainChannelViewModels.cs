using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.OffchainNotifcations;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainGroupViewModel
    {
        protected OffchainGroupViewModel(IGroup group, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            AssetViewModel asset;
            if (group.IsColored)
            {
                asset = assetDictionary.ContainsKey(group.AssetId) ?
                    AssetViewModel.Create(assetDictionary[group.AssetId]) :
                    AssetViewModel.CreateNotFoundAsset(group.AssetId);
            }
            else
            {
                asset = AssetViewModel.BtcAsset.Value;
            }

            var offchainTransactions =group.Transactions.Where(p=>p.OffchainTransactionData!=null).Select(p=> OffchainTransactionViewModel.Create(p.OffchainTransactionData));
            
            this.OffChainTransactions = offchainTransactions;
            this.Asset = asset;
        }

        public AssetViewModel Asset { get; set; }

        public IEnumerable<OffchainTransactionViewModel> OffChainTransactions { get; set; }

        public static OffchainGroupViewModel Create(IGroup group, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return new OffchainGroupViewModel(group, assetDictionary);
        }
    }

    public class OffchainTransactionViewModel
    {
        public string TransactionId { get;  set; }

        public DateTime DateTime { get;  set; }

        public string HubAddress { get;  set; }

        public string Address1 { get;  set; }

        public string Address2 { get;  set; }

        public string AssetId { get;  set; }

        public bool IsColored { get;  set; }

        public decimal Address1Quantity { get;  set; }
        public decimal Address1QuantityDiff { get; set; }
        

        public decimal Address1QuanrtityPercents => Math.Round((Address1Quantity / TotalQuantity) * 100);
        public decimal Address2Quantity { get;  set; }
        public decimal Address2QuantityDiff { get; set; }
        public decimal Address2QuanrtityPercents => Math.Round((Address2Quantity / TotalQuantity) * 100);

        public decimal TotalQuantity => Address1Quantity + Address2Quantity;

        public static OffchainTransactionViewModel Create(IOffchainTransaction source)
        {
            return new OffchainTransactionViewModel
            {
                AssetId = source.AssetId,
                Address1 = source.Address1,
                IsColored = source.IsColored,
                DateTime = source.DateTime,
                HubAddress = source.HubAddress,
                Address1Quantity = source.Address1Quantity,
                Address2 = source.Address2,
                Address2Quantity = source.Address2Quantity,
                TransactionId = source.TransactionId,
                Address1QuantityDiff = source.Address1QuantityDiff,
                Address2QuantityDiff = source.Address2QuantityDiff
            };
        }
        
    }
}