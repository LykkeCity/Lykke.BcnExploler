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
using Lykke.Service.BcnExploler.Core.Helpers;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TriggerHandlers
{
    public class AssetDataCommandQueueConsumer
    {
        private readonly ILog _log;
        private readonly IAssetDefinitionReader _assetDefinitionReader;
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly IAssetImageCommandProducer _assetImageCommandProducer;
	    private readonly IConsole _console;

        public AssetDataCommandQueueConsumer(ILog log, 
            IAssetDefinitionReader assetDefinitionReader, 
            IAssetDefinitionRepository assetDefinitionRepository,
            IAssetImageCommandProducer assetImageCommandProducer, 
			IConsole console)
        {
            _log = log;
            _assetDefinitionReader = assetDefinitionReader;
            _assetDefinitionRepository = assetDefinitionRepository;
            _assetImageCommandProducer = assetImageCommandProducer;
	        _console = console;
        }


        [QueueTrigger(QueueNames.AssetDefinitionScanner.RetrieveAsset)]
        public async Task RetrieveAssetDefinition(UpdateAssetDataContext context)
        {
            try
            {
                _console.Write(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), "Started");
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
                else
                {
                    await _assetDefinitionRepository.InsertEmptyAsync(context.AssetDefinitionUrl);
                }

	            _console.Write(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), "Done");
            }
            catch (Exception e)
            {
                await _log.WriteWarningAsync(nameof(AssetDataCommandQueueConsumer), nameof(RetrieveAssetDefinition), context.ToJson(), e.ToString());

                if (!await _assetDefinitionRepository.IsAssetExistsAsync(context.AssetDefinitionUrl))
                {
                    await _assetDefinitionRepository.InsertEmptyAsync(context.AssetDefinitionUrl);
                }
            }

        }
    }
}
