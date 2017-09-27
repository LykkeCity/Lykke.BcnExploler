using System;
using System.Threading.Tasks;
using AzureStorage.Queue;
using Common;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Commands;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Commands
{
    public class AssetDefinitionParseBlockCommandProducer: IAssetDefinitionParseBlockCommandProducer
    {
        private readonly IQueueExt _queue;

        public AssetDefinitionParseBlockCommandProducer(IQueueExt queue)
        {
            _queue = queue;
        }


        public Task CreateParseBlockCommand(string blockHash)
        {
            var context = new AssetDefinitionParseBlockContext
            {
                BlockHash = blockHash
            };
            return _queue.PutRawMessageAsync(context.ToJson());
        }
    }
}
