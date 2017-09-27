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

        public AssetIndexFunctions(IAssetCoinholdersIndexesCommandProducer assetCoinholdersIndexesCommandProducer,
            IAssetDefinitionRepository assetDefinitionRepository)
        {
            _assetCoinholdersIndexesCommandProducer = assetCoinholdersIndexesCommandProducer;
            _assetDefinitionRepository = assetDefinitionRepository;
        }

        [TimerTrigger("23:00:00")]
        public async Task UpdateIndexCoinholdersData()
        {
            var assets = await _assetDefinitionRepository.GetAllAsync();
            await _assetCoinholdersIndexesCommandProducer.CreateAssetCoinholdersUpdateIndexCommand(assets.ToArray());
        }
    }
}
