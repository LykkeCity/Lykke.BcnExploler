﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Web.Models.Asset;
using Lykke.Service.BcnExploler.Web.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Lykke.Service.BcnExploler.Web.Models.Offchain
{
    public class OffchainFilledChannelViewModel: OffchainChannelViewModel
    {
        private OffchainFilledChannelViewModel(IFilledChannel channel, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary):base(channel, assetDictionary)
        {
            this.OpenTransaction = TransactionViewModel.Create(channel.OpenFilledTransaction, assetDictionary, channel.OpenTransaction?.Type);
            this.CloseTransaction = TransactionViewModel.Create(channel.CloseFilledTransaction, assetDictionary, channel.CloseTransaction?.Type);
        }

        public TransactionViewModel OpenTransaction { get; set; }

        public TransactionViewModel CloseTransaction { get; set; }

        public static OffchainFilledChannelViewModel Create(IFilledChannel channel, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return new OffchainFilledChannelViewModel(channel, assetDictionary);
        }
    }

    public class OffchainChannelViewModel
    {
        protected OffchainChannelViewModel(IChannel channel, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            AssetViewModel asset;
            if (channel.IsColored)
            {
                asset = assetDictionary.ContainsKey(channel.AssetId) ?
                    AssetViewModel.Create(assetDictionary[channel.AssetId]) :
                    AssetViewModel.CreateNotFoundAsset(channel.AssetId);
            }
            else
            {
                asset = AssetViewModel.BtcAsset.Value;
            }

            var offchainTransactions =
                channel.OffchainTransactions.OrderByDescending(p => p.DateTime).Select(OffchainTransactionViewModel.Create);

            this.OpenTransactionTd = channel.OpenTransaction?.TransactionId;
            this.CloseTransactionId = channel.CloseTransaction?.TransactionId;
            this.OffChainTransactions = offchainTransactions;
            this.Asset = asset;
        }

        public AssetViewModel Asset { get; set; }
        public string OpenTransactionTd { get; set; }

        public string CloseTransactionId { get; set; }

        public IEnumerable<OffchainTransactionViewModel> OffChainTransactions { get; set; }

        public static OffchainChannelViewModel Create(IChannel channel, IReadOnlyDictionary<string, IAssetDefinition> assetDictionary)
        {
            return new OffchainChannelViewModel(channel, assetDictionary);
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

        public decimal Address2Quantity { get;  set; }

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
                TransactionId = source.TransactionId
            };
        }
        
    }
}