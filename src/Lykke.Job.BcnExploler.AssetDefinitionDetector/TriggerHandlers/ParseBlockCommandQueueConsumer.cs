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
using Lykke.Service.BcnExploler.Core.Helpers;
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
	    private readonly IConsole _console;

        public ParseBlockCommandQueueConsumer(ILog log, 
            IAssetDefinitionParsedBlockRepository assetDefinitionParsedBlockRepository, 
            IAssetDefinitionCommandProducer assetDefinitionCommandProducer, 
            IndexerClient indexerClient, IConsole console)
        {
            _log = log;
            _assetDefinitionParsedBlockRepository = assetDefinitionParsedBlockRepository;
            _assetDefinitionCommandProducer = assetDefinitionCommandProducer;
            _indexerClient = indexerClient;
	        _console = console;
        }

        [QueueTrigger(QueueNames.AssetDefinitionScanner.ParseBlock, notify: true)]
        public async Task ParseBlock(AssetDefinitionParseBlockContext context)
        {
            try
            {
                _console.Write(nameof(ParseBlockCommandQueueConsumer), nameof(ParseBlock), context.ToJson(), "Started");
                var block = await _indexerClient.GetBlock(uint256.Parse(context.BlockHash));

                foreach (var transaction in block.Transactions.Where(p => p.HasValidColoredMarker()))
                {
                    var assetDefUrl = transaction.TryGetAssetDefinitionUrl();

                    if (assetDefUrl != null)
                    {
                        await _log.WriteInfoAsync(nameof(ParseBlockCommandQueueConsumer),
                            nameof(ParseBlock), 
                            context.ToJson(),
                            $"Found asset definition url {assetDefUrl.AbsoluteUri}");

                        await _assetDefinitionCommandProducer.CreateRetrieveAssetDefinitionCommand(assetDefUrl.AbsoluteUri);
                    }
                }
                

                await _assetDefinitionParsedBlockRepository.AddBlockAsync(AssetDefinitionParsedBlock.Create(context.BlockHash));

	            _console.Write(nameof(ParseBlockCommandQueueConsumer), nameof(ParseBlock), context.ToJson(), "Done");
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(ParseBlockCommandQueueConsumer), nameof(ParseBlock), context.ToJson(), e);
            }
        }
        
    }
}
