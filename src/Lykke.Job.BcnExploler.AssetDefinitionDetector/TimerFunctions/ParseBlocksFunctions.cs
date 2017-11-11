using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Helpers;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TimerFunctions
{
    public class ParseBlocksFunctions
    {
        private readonly ILog _log;
        private readonly IAssetDefinitionParseBlockCommandProducer _parseBlockCommandProducer;
        private readonly IAssetDefinitionParsedBlockRepository _assetDefinitionParsedBlockRepository;
        private readonly IBlockService _blockService;
	    private readonly IConsole _console;

        public ParseBlocksFunctions(ILog log, 
            IAssetDefinitionParsedBlockRepository assetDefinitionParsedBlockRepository,
            IBlockService blockService, 
            IAssetDefinitionParseBlockCommandProducer parseBlockCommandProducer, IConsole console)
        {
            _log = log;
            _assetDefinitionParsedBlockRepository = assetDefinitionParsedBlockRepository;
            _blockService = blockService;
            _parseBlockCommandProducer = parseBlockCommandProducer;
	        _console = console;
        }

        [TimerTrigger("00:10:00")]
        public async Task ParseLast()
        {
            IBlockHeader blockPtr = null;

            try
            {
	            _console.Write(nameof(ParseBlocksFunctions), nameof(ParseLast), null, "Started");

                blockPtr = await _blockService.GetLastBlockHeaderAsync();
                
                while (blockPtr != null && 
                    !await _assetDefinitionParsedBlockRepository
                        .IsBlockExistsAsync(AssetDefinitionParsedBlock.Create(blockPtr.Hash)))
                {
	                _console.Write(nameof(ParseBlocksFunctions),
                        nameof(ParseLast),
                        new {blockPtr.Hash, blockPtr.Height}.ToJson(),
                        $"Add parse block command {blockPtr.Height}");

                    await _parseBlockCommandProducer.CreateParseBlockCommand(blockPtr.Hash);
                    
                    blockPtr = await _blockService.GetBlockHeaderAsync((blockPtr.Height - 1).ToString());
                }

                _console.Write(nameof(ParseBlocksFunctions), nameof(ParseLast), null, "Done");

            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(ParseBlocksFunctions), 
                    nameof(ParseLast), 
                    new { blockHash = blockPtr?.Hash }.ToJson(),
                    e);

                throw;
            }
        }
    }
}
