using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Core.Transaction;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;
using Lykke.Service.BcnExploler.Core.Asset.Indexes.Commands;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Core.Helpers;
using Lykke.Service.BcnExploler.Core.MainChain;
using Lykke.Service.BcnExploler.Services.Helpers;

namespace Lykke.Job.BcnExploler.AssetIndexer.TriggerHandlers
{
    public class AssetCoinholderIndexesCommandsQueueConsumer 
    {
        private readonly ILog _log;
        private readonly IAssetCoinholdersIndexRepository _assetCoinholdersIndexRepository;
        private readonly IAssetBalanceChangesRepository _balanceChangesRepository;
        private readonly IAssetService _assetService;
        private readonly ITransactionService _transactionService;
        private readonly IMainChainService _mainChainService;
	    private readonly IConsole _console;

        public AssetCoinholderIndexesCommandsQueueConsumer(ILog log, 
            IAssetCoinholdersIndexRepository assetCoinholdersIndexRepository, 
            IAssetBalanceChangesRepository balanceChangesRepository, 
            IAssetService assetService, 
            ITransactionService transactionService, 
            IMainChainService mainChainService,
			IConsole console)
        {
            _log = log;
            _assetCoinholdersIndexRepository = assetCoinholdersIndexRepository;
            _balanceChangesRepository = balanceChangesRepository;
            _assetService = assetService;
            _transactionService = transactionService;
            _mainChainService = mainChainService;
	        _console = console;
        }


        [QueueTrigger(QueueNames.AssetIndexer.UpdateIndex)]
        public async Task UpdateCoinholersIndex(AssetCoinholdersUpdateIndexCommand context)
        {
            try
            {
                _console.Write(nameof(AssetCoinholderIndexesCommandsQueueConsumer), nameof(UpdateCoinholersIndex),
                    context.ToJson(), "Started");

                var asset = await _assetService.GetAssetAsync(context.AssetId);
                if (asset != null && asset.IsValid())
                {
                    var mainChain = await _mainChainService.GetMainChainAsync();

                    var balanceSummary = _balanceChangesRepository.GetSummaryAsync(asset.AssetIds.ToArray());
                    var blocksWithChanges = _balanceChangesRepository.GetBlocksWithChanges(asset.AssetIds);
                    var allTxs = _balanceChangesRepository.GetTransactionsAsync(asset.AssetIds);
                    var monthAgoBlock = mainChain.GetClosestToTimeBlock(DateTime.Now.AddDays(-30));
                    var lastMonthTxs = _balanceChangesRepository.GetTransactionsAsync(asset.AssetIds,
                        monthAgoBlock?.Height);

                    var lastTxDate = _balanceChangesRepository.GetLatestTxAsync(asset.AssetIds)
                        .ContinueWith(async p => (await _transactionService.GetAsync(p.Result?.Hash))?.Block?.Time);

                    await Task.WhenAll(balanceSummary, blocksWithChanges, allTxs, lastTxDate.Unwrap(), lastMonthTxs);

                    await _assetCoinholdersIndexRepository.InserOrReplaceAsync(
                            AssetCoinholdersIndex.Create(balanceSummary.Result, 
                                blocksWithChanges.Result, 
                                allTxs.Result.Count(), lastMonthTxs.Result.Count(), lastTxDate.Unwrap().Result));
                }

	            _console.Write(nameof(AssetCoinholderIndexesCommandsQueueConsumer), nameof(UpdateCoinholersIndex),
                    context.ToJson(), "Done");
            }
            catch (Exception e)
            {
                await  _log.WriteErrorAsync(nameof(AssetCoinholderIndexesCommandsQueueConsumer), nameof(UpdateCoinholersIndex),
                   context.ToJson(), e);
            }
        }
    }
}
