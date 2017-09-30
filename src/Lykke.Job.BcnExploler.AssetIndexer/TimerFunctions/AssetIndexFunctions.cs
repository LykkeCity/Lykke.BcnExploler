using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Indexes.Commands;

namespace Lykke.Job.BcnExploler.AssetIndexer.TimerFunctions
{
    public class AssetIndexFunctions
    {
        private readonly IAssetCoinholdersIndexesCommandProducer _assetCoinholdersIndexesCommandProducer;
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly ILog _log;

        public AssetIndexFunctions(IAssetCoinholdersIndexesCommandProducer assetCoinholdersIndexesCommandProducer,
            IAssetDefinitionRepository assetDefinitionRepository, 
            ILog log)
        {
            _assetCoinholdersIndexesCommandProducer = assetCoinholdersIndexesCommandProducer;
            _assetDefinitionRepository = assetDefinitionRepository;
            _log = log;
        }

        [TimerTrigger("23:00:00")]
        public async Task UpdateIndexCoinholdersData()
        {
            await _log.WriteInfoAsync(nameof(AssetIndexFunctions), nameof(UpdateIndexCoinholdersData), null, "Started");

            var assets = await _assetDefinitionRepository.GetAllAsync();
            await _assetCoinholdersIndexesCommandProducer.CreateAssetCoinholdersUpdateIndexCommand(assets.ToArray());

            await _log.WriteInfoAsync(nameof(AssetIndexFunctions), nameof(UpdateIndexCoinholdersData), null, "Done");
        }
    }
}
