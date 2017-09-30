using System;
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
        private readonly IAssetDefinitionReader _assetDefinitionReader;
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly IAssetImageCommandProducer _assetImageCommandProducer;

        public AssetDataCommandQueueConsumer(ILog log, 
            IAssetDefinitionReader assetDefinitionReader, 
            IAssetDefinitionRepository assetDefinitionRepository,
            IAssetImageCommandProducer assetImageCommandProducer)
        {
            _log = log;
            _assetDefinitionReader = assetDefinitionReader;
            _assetDefinitionRepository = assetDefinitionRepository;
            _assetImageCommandProducer = assetImageCommandProducer;
        }


        [QueueTrigger(QueueNames.AssetDefinitionScanner.RetrieveAsset)]
        public async Task RetrieveAssetDefinition(UpdateAssetDataContext context)
        {
            try
            {
                await _log.WriteInfoAsync(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), "Started");
                var assetData = await _assetDefinitionReader.ReadAssetDataAsync(context.AssetDefinitionUrl);
                if (assetData != null)
                {
                    await _assetDefinitionRepository.InsertOrReplaceAsync(assetData);
                    if (assetData.IsValid())
                    {
                        await _assetImageCommandProducer.CreateUpsertAssetImageCommand(assetData.AssetIds, assetData.IconUrl,
                                assetData.ImageUrl);
                    }
                }

                await _log.WriteInfoAsync(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), "Done");
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), e);

                if (!await _assetDefinitionRepository.IsAssetExistsAsync(context.AssetDefinitionUrl))
                {
                    await _assetDefinitionRepository.InsertEmptyAsync(context.AssetDefinitionUrl);
                }
            }

        }
    }
}
