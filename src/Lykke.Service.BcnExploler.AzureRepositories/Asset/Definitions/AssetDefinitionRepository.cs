﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Definitions
{
    public class AssetDefinitionDefinitionEntity : TableEntity, IAssetDefinition
    {
        public static string GeneratePartitionKey()
        {
            return "Asset";
        }

        public static string GenerateEmptyPartitionKey()
        {
            return "Failed Create Asset";
        }

        public static string GenerateRowKey(IEnumerable<string> assetIds)
        {
            return string.Join("_", assetIds);
        }

        IEnumerable<string> IAssetDefinition.AssetIds => Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(AssetIds);
        public string AssetIds { get; set; }
        public string ContactUrl { get; set; }
        public string NameShort { get; set; }
        public string Name { get; set; }
        public string Issuer { get; set; }
        public string Description { get; set; }
        public string DescriptionMime { get; set; }
        public string Type { get; set; }
        public int Divisibility { get; set; }
        public bool LinkToWebsite { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Version { get; set; }
        public string AssetDefinitionUrl { get; set; }

        public static AssetDefinitionDefinitionEntity Create(IAssetDefinition data)
        {
            return new AssetDefinitionDefinitionEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(data.AssetIds),
                AssetDefinitionUrl = data.AssetDefinitionUrl,
                ContactUrl = data.ContactUrl,
                NameShort = data.NameShort,
                Name = data.Name,
                Description = data.Description,
                DescriptionMime = data.DescriptionMime,
                Divisibility = data.Divisibility,
                IconUrl = data.IconUrl,
                ImageUrl = data.ImageUrl,
                Issuer = data.Issuer,
                LinkToWebsite = data.LinkToWebsite,
                Type = data.Type,
                Version = data.Version,
                AssetIds = data.AssetIds.ToJson()
            };
        }

        public static AssetDefinitionDefinitionEntity CreateEmpty(string url)
        {
            return new AssetDefinitionDefinitionEntity
            {
                PartitionKey = GenerateEmptyPartitionKey(),
                RowKey = Guid.NewGuid().ToString(),
                AssetDefinitionUrl = url
            };
        }
    }


    public class AssetDefinitionRepository : IAssetDefinitionRepository
    {
        private readonly INoSQLTableStorage<AssetDefinitionDefinitionEntity> _assetTableStorage;

        public AssetDefinitionRepository(INoSQLTableStorage<AssetDefinitionDefinitionEntity> assetTableStorage)
        {
            _assetTableStorage = assetTableStorage;
        }

        public async Task<IEnumerable<IAssetDefinition>> GetAllEmptyAsync()
        {
            return await _assetTableStorage.GetDataAsync(AssetDefinitionDefinitionEntity.GenerateEmptyPartitionKey());
        }


        public async Task DeleteEmptyAssets()
        {
            var empty = await _assetTableStorage.GetDataAsync(AssetDefinitionDefinitionEntity.GenerateEmptyPartitionKey());

            await _assetTableStorage.DeleteAsync(empty);
        }
        public async Task InsertOrReplaceAsync(IAssetDefinition[] assetsDefinition)
        {
            await _assetTableStorage.InsertOrReplaceBatchAsync(assetsDefinition.Select(AssetDefinitionDefinitionEntity.Create));
        }

        public async Task InsertEmptyAsync(string defUrl)
        {
            await _assetTableStorage.InsertOrReplaceAsync(AssetDefinitionDefinitionEntity.CreateEmpty(defUrl));
        }

        public async Task<IEnumerable<IAssetDefinition>> GetAllAsync()
        {
            return await _assetTableStorage.GetDataAsync(AssetDefinitionDefinitionEntity.GeneratePartitionKey());
        }

        public async Task<bool> IsAssetExistsAsync(string defUrl)
        {
            return (await _assetTableStorage.GetDataAsync(p => p.AssetDefinitionUrl == defUrl)).Any();
        }

        public async Task RemoveEmptyAsync(params string[] defUrls)
        {
            foreach (var defUrl in defUrls)
            {
                await _assetTableStorage.DeleteAsync(AssetDefinitionDefinitionEntity.CreateEmpty(defUrl));
            }
        }

        public async Task UpdateAssetAsync(IAssetDefinition assetDefinition)
        {
            await _assetTableStorage.InsertOrReplaceAsync(AssetDefinitionDefinitionEntity.Create(assetDefinition));
        }
    }
}
