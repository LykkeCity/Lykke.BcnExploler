using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Common;
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
            await _queue.PutRawMessageAsync(new AssetImageContext
            {
                AssetIds = assetIds,
                IconUrl = iconUrl,
                ImageUrl = imageUrl
            }.ToJson());
        }
    }
}
