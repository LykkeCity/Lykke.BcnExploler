using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Definitions.Commands
{
    public class AssetImageCommandProducer: IAssetImageCommandProducer
    {
        private readonly IQueueExt _queue;

        public AssetImageCommandProducer(IQueueExt queue)
        {
            _queue = queue;
        }

        public async Task CreateUpsertAssetImageCommand(IEnumerable<string> assetIds, string iconUrl, string imageUrl)
        {
            await _queue.PutMessageAsync(new QueueRequestModel<AssetImageContext>
            {
                Data = new AssetImageContext
                {
                    AssetIds = assetIds,
                    IconUrl = iconUrl,
                    ImageUrl = imageUrl
                }
            });
        }
    }
}
