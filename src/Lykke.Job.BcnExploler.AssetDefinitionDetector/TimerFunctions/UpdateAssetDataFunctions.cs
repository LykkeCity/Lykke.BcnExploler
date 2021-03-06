﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;
using Lykke.Service.BcnExploler.Core.Helpers;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TimerFunctions
{
    public class UpdateAssetDataFunctions
    {
        private readonly IAssetDefinitionRepository _assetDefinitionRepository;
        private readonly ILog _log;
        private readonly IAssetDefinitionCommandProducer _assetDefinitionCommandProducer;
	    private readonly IConsole _console;

        public UpdateAssetDataFunctions(IAssetDefinitionRepository assetDefinitionRepository, 
            ILog log, 
            IAssetDefinitionCommandProducer assetDefinitionCommandProducer, 
			IConsole console)
        {
            _assetDefinitionRepository = assetDefinitionRepository;
            _log = log;
            _assetDefinitionCommandProducer = assetDefinitionCommandProducer;
	        _console = console;
        }

        [TimerTrigger("23:00:00")]
        public async Task UpdateAssets()
        {
            try
            {
                _console.Write(nameof(UpdateAssetDataFunctions), nameof(UpdateAssets), null, "Started");

                var assetsToUpdate = await _assetDefinitionRepository.GetAllAsync();

                var updUrls = assetsToUpdate.Select(p => p.AssetDefinitionUrl).ToArray();

                await _assetDefinitionCommandProducer.CreateRetrieveAssetDefinitionCommand(updUrls);

	            _console.Write(nameof(UpdateAssetDataFunctions), nameof(UpdateAssets), null, "Done");
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(UpdateAssetDataFunctions), nameof(UpdateAssets), null, e);
            }
        }

        [TimerTrigger("23:59:00")]
        public async Task UpdateFailedAssets()
        {
            try
            {
	            _console.Write(nameof(UpdateAssetDataFunctions), nameof(UpdateFailedAssets), null, "Started");

                var assetsToUpdate = await _assetDefinitionRepository.GetAllEmptyAsync();

                await _assetDefinitionRepository.DeleteEmptyAssets();
                var updUrls = assetsToUpdate.Select(p => p.AssetDefinitionUrl).ToArray();

                await _assetDefinitionCommandProducer.CreateRetrieveAssetDefinitionCommand(updUrls);

	            _console.Write(nameof(UpdateAssetDataFunctions), nameof(UpdateFailedAssets), null, "Done");
            }
            catch (Exception e)
            {
                await _log.WriteInfoAsync(nameof(UpdateAssetDataFunctions), nameof(UpdateFailedAssets), null, e.ToString());
            }
        }
    }
}
