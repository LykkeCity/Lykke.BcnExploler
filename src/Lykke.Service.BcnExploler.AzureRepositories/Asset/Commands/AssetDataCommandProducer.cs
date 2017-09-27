using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Commands;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Commands
{
    public class AssetDataCommandProducer: IAssetDataCommandProducer
    {
        private readonly IQueueExt _queue;

        public AssetDataCommandProducer(IQueueExt queue)
        {
            _queue = queue;
        }


        public async Task CreateUpdateAssetDataCommand(params string[] urls)
        {
            foreach (var url in urls)
            {
                await _queue.PutMessageAsync(new UpdateAssetDataContext
                {
                    AssetDefinitionUrl = url
                });
            }
        }
    }
}
