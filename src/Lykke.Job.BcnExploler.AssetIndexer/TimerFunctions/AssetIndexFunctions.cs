using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Indexes.Commands;
using Lykke.Service.BcnExploler.Core.Helpers;

namespace Lykke.Job.BcnExploler.AssetIndexer.TimerFunctions
{
    public class AssetIndexFunctions
    {
        private readonly IAssetCoinholdersIndexesCommandProducer _assetCoinholdersIndexesCommandProducer;
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly IConsole _console;

        public AssetIndexFunctions(IAssetCoinholdersIndexesCommandProducer assetCoinholdersIndexesCommandProducer,
            IAssetDefinitionRepository assetDefinitionRepository, IConsole console)
        {
            _assetCoinholdersIndexesCommandProducer = assetCoinholdersIndexesCommandProducer;
            _assetDefinitionRepository = assetDefinitionRepository;
	        _console = console;
        }

        [TimerTrigger("23:00:00")]
        public async Task UpdateIndexCoinholdersData()
        {
            _console.Write(nameof(AssetIndexFunctions), nameof(UpdateIndexCoinholdersData), null, "Started");

            var assets = await _assetDefinitionRepository.GetAllAsync();
            await _assetCoinholdersIndexesCommandProducer.CreateAssetCoinholdersUpdateIndexCommand(assets.ToArray());

	        _console.Write(nameof(AssetIndexFunctions), nameof(UpdateIndexCoinholdersData), null, "Done");
        }
    }
}
