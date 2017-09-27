using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Indexes.Commands;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Indexes.Commands
{
    public class AssetCoinholdersIndexesCommandProducer: IAssetCoinholdersIndexesCommandProducer
    {
        private readonly IQueueExt _queue;

        public AssetCoinholdersIndexesCommandProducer(IQueueExt queue)
        {
            _queue = queue;
        }

        public async Task CreateAssetCoinholdersUpdateIndexCommand(params IAssetDefinition[] assetsDefinition)
        {
            foreach (var asset in assetsDefinition)
            {
                await CreateAssetCoinholdersUpdateIndexCommand(asset.AssetIds.FirstOrDefault());
            }
        }

        public async Task CreateAssetCoinholdersUpdateIndexCommand(params string[] assetIds)
        {
            foreach (var assetID in assetIds)
            {
                await _queue.PutMessageAsync(new QueueRequestModel<AssetCoinholdersUpdateIndexCommand>
                {
                    Data = new AssetCoinholdersUpdateIndexCommand
                    {
                        AssetId = assetID
                    }
                });
            }
        }
    }
}
