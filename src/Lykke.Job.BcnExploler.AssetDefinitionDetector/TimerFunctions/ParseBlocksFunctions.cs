using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;
using Lykke.Service.BcnExploler.Core.Block;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TimerFunctions
{
    public class ParseBlocksFunctions
    {
        private readonly ILog _log;
        private readonly IAssetDefinitionParseBlockCommandProducer _parseBlockCommandProducer;
        private readonly IAssetDefinitionParsedBlockRepository _assetDefinitionParsedBlockRepository;
        private readonly IBlockService _blockService;

        public ParseBlocksFunctions(ILog log, 
            IAssetDefinitionParsedBlockRepository assetDefinitionParsedBlockRepository,
            IBlockService blockService, 
            IAssetDefinitionParseBlockCommandProducer parseBlockCommandProducer)
        {
            _log = log;
            _assetDefinitionParsedBlockRepository = assetDefinitionParsedBlockRepository;
            _blockService = blockService;
            _parseBlockCommandProducer = parseBlockCommandProducer;
        }

        [TimerTrigger("00:10:00")]
        public async Task ParseLast()
        {
            IBlockHeader blockPtr = null;

            try
            {
                blockPtr = await _blockService.GetLastBlockHeaderAsync();
                while (blockPtr != null && 
                    !await _assetDefinitionParsedBlockRepository
                        .IsBlockExistsAsync(AssetDefinitionParsedBlock.Create(blockPtr.Hash)))
                {
                    await _parseBlockCommandProducer.CreateParseBlockCommand(blockPtr.Hash);

                    blockPtr = await _blockService.GetBlockHeaderAsync((blockPtr.Height - 1).ToString());
                }
                
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("ParseBlocksFunctions", 
                    "ParseLast", 
                    new { blockHash = blockPtr?.Hash }.ToJson(),
                    e);

                throw;
            }
        }
    }
}
