using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;
using Lykke.Service.BcnExploler.Services.Helpers;

namespace Lykke.Job.BcnExploler.AssetIndexer.TimerFunctions
{
    public class AssetScoreFunctions
    {
        private readonly IAssetCoinholdersIndexRepository _indexRepository;
        private readonly IAssetScoreRepository _assetScoreRepository;
        private readonly ILog _log;
        private readonly IAssetService _assetService;
        
        public AssetScoreFunctions(IAssetCoinholdersIndexRepository indexRepository, 
            ILog log, 
            IAssetService assetService, 
            IAssetScoreRepository assetScoreRepository)
        {
            _indexRepository = indexRepository;
            _log = log;
            _assetService = assetService;
            _assetScoreRepository = assetScoreRepository;
        }

        [TimerTrigger("23:59:00")]
        public async Task UpdateAssetScores()
        {
            try
            {
                var indexes = (await _indexRepository.GetAllAsync()).ToList();
                foreach (var index in indexes)
                {
                    var score = AssetScoreHelper.CalculateAssetScore(await _assetService.GetAssetAsync(index.AssetIds.FirstOrDefault()), index, indexes);
                    
                    await _assetScoreRepository.InsertOrReplaceAsync(AssetScore.Create(index.AssetIds, score));
                }
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("AssetScoreFunctions", "UpdateAssetScores", null, e);
                throw;
            }
        }
    }
}
