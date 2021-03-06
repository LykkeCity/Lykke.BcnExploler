﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Indexes
{
    public class AssetCoinholdersIndexEntity:TableEntity, IAssetCoinholdersIndex
    {
        IEnumerable<string> IAssetCoinholdersIndex.AssetIds => JsonConvert.DeserializeObject<List<string>>(AssetIds);
        public int CoinholdersCount { get; set; }
        public double TotalQuantity { get; set; }
        public double TopCoinholderShare { get; set; }
        public double HerfindalShareIndex { get; set; }
        public DateTime? LastTxDate { get; set; }
        public int TransactionsCount { get; set; }
        public int LastMonthTransactionCount { get; set; }
        public string AssetIds { get; set; }

        public static string GenerateRowKey(IEnumerable<string> assetIds)
        {
            return string.Join("_", assetIds);
        }

        public static string GeneratePartitionKey()
        {
            return "ACI";
        }

        public static AssetCoinholdersIndexEntity Create(IAssetCoinholdersIndex source)
        {
            return new AssetCoinholdersIndexEntity
            {
                AssetIds = source.AssetIds.ToJson(),
                RowKey = GenerateRowKey(source.AssetIds),
                PartitionKey = GeneratePartitionKey(),
                CoinholdersCount = source.CoinholdersCount,
                TotalQuantity = source.TotalQuantity,
                LastTxDate = source.LastTxDate,
                HerfindalShareIndex = source.HerfindalShareIndex,
                TransactionsCount = source.TransactionsCount,
                TopCoinholderShare = source.TopCoinholderShare,
                LastMonthTransactionCount = source.LastMonthTransactionCount
            };
        }
    }

    public class AssetCoinholdersIndexRepository: IAssetCoinholdersIndexRepository
    {
        private readonly INoSQLTableStorage<AssetCoinholdersIndexEntity> _tableStorage;

        public AssetCoinholdersIndexRepository(INoSQLTableStorage<AssetCoinholdersIndexEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task InserOrReplaceAsync(IAssetCoinholdersIndex index)
        {
            await _tableStorage.InsertOrReplaceAsync(AssetCoinholdersIndexEntity.Create(index));
        }

        public async Task<IEnumerable<IAssetCoinholdersIndex>> GetAllAsync()
        {
            return await _tableStorage.GetDataAsync();
        }
    }
}
