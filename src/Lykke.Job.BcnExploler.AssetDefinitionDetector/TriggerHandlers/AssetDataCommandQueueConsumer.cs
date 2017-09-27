﻿using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TriggerHandlers
{
    public class AssetDataCommandQueueConsumer
    {
        private readonly ILog _log;
        private readonly IAssetReader _assetReader;
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly IAssetImageCommandProducer _assetImageCommandProducer;

        public AssetDataCommandQueueConsumer(ILog log, 
            IAssetReader assetReader, 
            IAssetDefinitionRepository assetDefinitionRepository,
            IAssetImageCommandProducer assetImageCommandProducer)
        {
            _log = log;
            _assetReader = assetReader;
            _assetDefinitionRepository = assetDefinitionRepository;
            _assetImageCommandProducer = assetImageCommandProducer;
        }


        [QueueTrigger(QueueNames.AssetDefinitionScanner.RetrieveAsset)]
        public async Task UpdateAssetData(UpdateAssetDataContext context)
        {
            try
            {
                var assetData = await _assetReader.ReadAssetDataAsync(context.AssetDefinitionUrl);
                if (assetData != null)
                {
                    await _assetDefinitionRepository.InsertOrReplaceAsync(assetData);
                    if (assetData.IsValid())
                    {
                        await _assetImageCommandProducer.CreateUpsertAssetImageCommand(assetData.AssetIds, assetData.IconUrl,
                                assetData.ImageUrl);
                    }

                }
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("UpdateAssetDataCommandQueueConsumer", "UpdateAssetData", context.ToJson(), e);

                if (!await _assetDefinitionRepository.IsAssetExistsAsync(context.AssetDefinitionUrl))
                {
                    await _assetDefinitionRepository.InsertEmptyAsync(context.AssetDefinitionUrl);
                }
            }

        }
    }
}
