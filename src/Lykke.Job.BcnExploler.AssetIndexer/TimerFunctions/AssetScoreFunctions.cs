using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;
using Lykke.Service.BcnExploler.Core.Helpers;
using Lykke.Service.BcnExploler.Services.Helpers;

namespace Lykke.Job.BcnExploler.AssetIndexer.TimerFunctions
{
    public class AssetScoreFunctions
    {
        private readonly IAssetCoinholdersIndexRepository _indexRepository;
        private readonly IAssetScoreRepository _assetScoreRepository;
        private readonly ILog _log;
        private readonly IAssetService _assetService;
	    private readonly IConsole _console;
        
        public AssetScoreFunctions(IAssetCoinholdersIndexRepository indexRepository, 
            ILog log, 
            IAssetService assetService, 
            IAssetScoreRepository assetScoreRepository, IConsole console)
        {
            _indexRepository = indexRepository;
            _log = log;
            _assetService = assetService;
            _assetScoreRepository = assetScoreRepository;
	        _console = console;
        }

        [TimerTrigger("23:59:00")]
        public async Task UpdateAssetScores()
        {
            try
            {
	            _console.Write(nameof(AssetScoreFunctions), nameof(UpdateAssetScores), null, "Started");
                var indexes = (await _indexRepository.GetAllAsync()).ToList();

                foreach (var index in indexes)
                {
                    var score = AssetScoreHelper.CalculateAssetScore(await _assetService.GetAssetAsync(index.AssetIds.FirstOrDefault()), index, indexes);
                    
                    await _assetScoreRepository.InsertOrReplaceAsync(AssetScore.Create(index.AssetIds, score));
                }

	            _console.Write(nameof(AssetScoreFunctions), nameof(UpdateAssetScores), null, "Done");
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(AssetScoreFunctions), nameof(UpdateAssetScores), null, e);
                throw;
            }
        }
    }
}
