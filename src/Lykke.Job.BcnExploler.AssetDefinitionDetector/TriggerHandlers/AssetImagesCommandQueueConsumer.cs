using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Images;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.TriggerHandlers
{
    public class AssetImagesCommandQueueConsumer
    {
        private readonly ILog _log;
        private readonly IAssetImageCacher _assetImageCacher;
        private readonly IAssetImageRepository _assetImageRepository;

        public AssetImagesCommandQueueConsumer(ILog log, IAssetImageCacher assetImageCacher, IAssetImageRepository assetImageRepository)
        {
            _log = log;
            _assetImageCacher = assetImageCacher;
            _assetImageRepository = assetImageRepository;
        }

        [QueueTrigger(QueueNames.AssetDefinitionScanner.UpsertImages)]
        public async Task UpdateAssetImage(AssetImageContext context)
        {
            try
            {
                var iconResult = await _assetImageCacher.SaveAssetIconAsync(context.IconUrl, context.AssetIds.First());
                var imageResult = await _assetImageCacher.SaveAssetImageAsync(context.ImageUrl, context.AssetIds.First());

                await _assetImageRepository.InsertOrReplaceAsync(
                    AssetImage.Create(context.AssetIds, 
                    iconResult,
                    imageResult));
            
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync("UpdateAssetImagesCommandQueueConsumer", "UpdateAssetImage", context.ToJson(), e);
            }
        }
    }
}
