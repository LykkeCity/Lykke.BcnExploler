using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;
using Lykke.Service.BcnExploler.Services.Helpers;
using Lykke.Service.BcnExploler.Services.Ninja;
using NBitcoin;
using NBitcoin.OpenAsset;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TriggerHandlers
{
    public class ParseBlockCommandQueueConsumer
    {
        private readonly ILog _log;
        private readonly IAssetDefinitionParsedBlockRepository _assetDefinitionParsedBlockRepository;
        private readonly IAssetDefinitionCommandProducer _assetDefinitionCommandProducer;
        private readonly IndexerClient _indexerClient;

        public ParseBlockCommandQueueConsumer(ILog log, 
            IAssetDefinitionParsedBlockRepository assetDefinitionParsedBlockRepository, 
            IAssetDefinitionCommandProducer assetDefinitionCommandProducer, 
            IndexerClient indexerClient)
        {
            _log = log;
            _assetDefinitionParsedBlockRepository = assetDefinitionParsedBlockRepository;
            _assetDefinitionCommandProducer = assetDefinitionCommandProducer;
            _indexerClient = indexerClient;
        }

        [QueueTrigger(QueueNames.AssetDefinitionScanner.ParseBlock, notify: true)]
        public async Task ParseBlock(AssetDefinitionParseBlockContext context)
        {
            try
            {
                var block = await _indexerClient.GetBlock(uint256.Parse(context.BlockHash));

                foreach (var transaction in block.Transactions.Where(p => p.HasValidColoredMarker()))
                {
                    var assetDefUrl = transaction.TryGetAssetDefinitionUrl();

                    if (assetDefUrl != null)
                    {
                        await _assetDefinitionCommandProducer.CreateRetrieveAssetDefinitionCommand(assetDefUrl.AbsoluteUri);
                    }
                }
                

                await _assetDefinitionParsedBlockRepository.AddBlockAsync(AssetDefinitionParsedBlock.Create(context.BlockHash));
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("ParseBlockCommandQueueConsumer", "ParseBlock", context.ToJson(), e);
            }
        }
        
    }
}
