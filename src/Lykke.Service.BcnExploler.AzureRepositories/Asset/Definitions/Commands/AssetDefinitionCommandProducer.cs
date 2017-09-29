using System.Threading.Tasks;
using AzureStorage.Queue;
using Common;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands;

namespace Lykke.Service.BcnExploler.AzureRepositories.Asset.Definitions.Commands
{
    public class AssetDefinitionCommandProducer: IAssetDefinitionCommandProducer
    {
        private readonly IQueueExt _queue;

        public AssetDefinitionCommandProducer(IQueueExt queue)
        {
            _queue = queue;
        }


        public async Task CreateRetrieveAssetDefinitionCommand(params string[] urls)
        {
            foreach (var url in urls)
            {
                var context = new UpdateAssetDataContext
                {
                    AssetDefinitionUrl = url
                };

                await _queue.PutRawMessageAsync(context.ToJson());
            }
        }
    }
}
