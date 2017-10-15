using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Web.Models.Asset;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{

    public class OffchainTransactionDetailsViewModel
    {
        public OffchainFilledChannelViewModel FilledChannel { get; set; }

        public OffChainTransactionViewModel Transaction { get; set; }

        public static OffchainTransactionDetailsViewModel Create(OffchainFilledChannelViewModel filledChannel,
            string transactionId)
        {
            return new OffchainTransactionDetailsViewModel
            {
                FilledChannel = filledChannel,
                Transaction = filledChannel.OffChainTransactions.First(x => x.TransactionId == transactionId)
            };
        }
    }

    public class OffChainTransactionViewModel
    {
        public string TransactionId { get; set; }
        public DateTime DateTime { get; set; }


        public string HubAddress { get; set; }

        public string Address1 { get; set; }


        public string Address2 { get; set; }

        public AssetViewModel Asset { get; set; }


        public bool IsRevoked { get; set; }

        public decimal Address1Quantity { get; set; }
        public decimal Address1QuanrtityPercents => Math.Round((Address1Quantity / TotalQuantity) * 100);

        public decimal Address2Quantity { get; set; }

        public decimal Address2QuanrtityPercents => Math.Round((Address2Quantity / TotalQuantity) * 100);
        public int InputCount => 1;
        public int OutputCount => 2;


        public decimal Address1QuantityDiff { get; set; }
        public decimal Address2QuantityDiff { get; set; }
        public decimal TotalQuantity => Address1Quantity + Address2Quantity;

        public static OffChainTransactionViewModel Create(string transactionId,
            AssetViewModel asset,
            DateTime dateTime,
            bool isRevoked,
            string hubAddress,
            string address1,
            string address2,
            decimal address1Quantity,
            decimal address2Quantity,
            decimal address1QuantityDiff,
            decimal address2QuantityDiff)
        {
            return new OffChainTransactionViewModel
            {
                TransactionId = transactionId,
                Address1 = address1,
                Address2 = address2,
                Address1Quantity = address1Quantity,
                Address2Quantity = address2Quantity,
                DateTime = dateTime,
                HubAddress = hubAddress,
                Asset = asset,
                IsRevoked = isRevoked,
                Address1QuantityDiff = address1QuantityDiff,
                Address2QuantityDiff = address2QuantityDiff
            };
        }

        public static OffChainTransactionViewModel Create(IOffchainTransaction tx,
            IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            if (tx == null)
            {
                return null;
            }


            AssetViewModel asset;
            if (tx.IsColored)
            {
                asset = assetDictionary.ContainsKey(tx.AssetId) ?
                    AssetViewModel.Create(assetDictionary[tx.AssetId]) :
                    AssetViewModel.CreateNotFoundAsset(tx.AssetId);
            }
            else
            {
                asset = AssetViewModel.BtcAsset.Value;
            }



            return OffChainTransactionViewModel.Create(
                transactionId: tx.TransactionId,
                address1: tx.Address1,
                asset: asset,
                dateTime: tx.DateTime,
                isRevoked: tx.IsRevoked,
                hubAddress: tx.HubAddress,
                address2: tx.Address2,
                address1Quantity: tx.Address1Quantity,
                address1QuantityDiff: tx.Address1QuantityDiff,
                address2Quantity: tx.Address2Quantity,
                address2QuantityDiff: tx.Address2QuantityDiff);
        }
    }
}
