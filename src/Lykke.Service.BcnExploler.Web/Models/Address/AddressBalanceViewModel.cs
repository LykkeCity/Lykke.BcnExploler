﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Web.Models.Asset;

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

        public IEnumerable<Asset> Assets { get; set; }
        public AssetDictionary AssetDic { get; set; }

        public DateTime LastBlockDateTime { get; set; }
        public int LastBlockHeight { get; set; }

        public DateTime AtBlockDateTime { get; set; }
        public int AtBlockHeight { get; set; }

        public bool ShowNext => NextBlock <= LastBlockHeight;
        public bool ShowPrev => PrevBlock >= 0;
        public int PrevBlock => AtBlockHeight - 1;
        public int NextBlock => AtBlockHeight + 1;

        public static AddressBalanceViewModel Create(IAddressBalance balance, IDictionary<string, IAssetDefinition> assetDictionary, IBlockHeader lastBlock, IBlockHeader atBlock)
        {
            return new AddressBalanceViewModel
            {
                AddressId = balance.AddressId,
                TotalConfirmedTransactions = balance.TotalTransactions,
                Balance = balance.BtcBalance,
                Assets = (balance.ColoredBalances??Enumerable.Empty<IColoredBalance>()).Select(p => new Asset
                {
                    AssetId = p.AssetId,
                    Quantity = p.Quantity,
                    UnconfirmedQuantityDelta = p.UnconfirmedQuantityDelta
                }),
                UnconfirmedBalanceDelta = balance.UnconfirmedBalanceDelta,
                AssetDic = AssetDictionary.Create(assetDictionary),
                LastBlockHeight = lastBlock.Height,
                LastBlockDateTime = lastBlock.Time,
                AtBlockHeight = (atBlock ?? lastBlock).Height,
                AtBlockDateTime = (atBlock ?? lastBlock).Time,
                TotalTransactionsCountCalculated = balance.TotalTransactionsCountCalculated,
                TotalSpendedTransactions = balance.TotalSpendedTransactions,
                TotalReceivedTransactions = balance.TotalReceivedTransactions,
                TotalReceivedTransactionsCountCalculated = balance.TotalReceivedTransactionsCountCalculated,
                TotalSpendedTransactionsCountCalculated = balance.TotalSpendedTransactionsCountCalculated
            };
        }
        
        public class Asset
        {
            public string AssetId { get; set; }
            public double Quantity { get; set; }
            public double UnconfirmedQuantityDelta { get; set; }
            public bool ShowUnconfirmedBalance => UnconfirmedQuantityDelta != 0;
            public double UnconfirmedQuantity => Quantity + UnconfirmedQuantityDelta;

            public bool ShowAsset => Quantity != 0 || UnconfirmedQuantity != 0;
        }
    }


}