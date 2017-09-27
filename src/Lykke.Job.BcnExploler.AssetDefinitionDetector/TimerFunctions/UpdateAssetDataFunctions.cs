using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TimerFunctions
{
    public class UpdateAssetDataFunctions
    {
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly ILog _log;
        private readonly IAssetDataCommandProducer _assetDataCommandProducer;

        public UpdateAssetDataFunctions(IAssetDefinitionRepository assetDefinitionRepository, 
            ILog log, 
            IAssetDataCommandProducer assetDataCommandProducer)
        {
            _assetDefinitionRepository = assetDefinitionRepository;
            _log = log;
            _assetDataCommandProducer = assetDataCommandProducer;
        }

        [TimerTrigger("23:00:00")]
        public async Task UpdateAssets()
        {
            try
            {
                var assetsToUpdate = await _assetDefinitionRepository.GetAllAsync();

                var updUrls = assetsToUpdate.Select(p => p.AssetDefinitionUrl).ToArray();

                await _assetDataCommandProducer.CreateUpdateAssetDataCommand(updUrls);
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("UpdateAssetDataFunctions", "UpdateAssets", null, e);
            }
        }
    }
}
