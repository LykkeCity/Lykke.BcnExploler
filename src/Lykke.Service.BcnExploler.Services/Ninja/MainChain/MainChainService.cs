using System;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.MainChain;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Ninja.MainChain
{
    public class MainChainService:IMainChainService
    {
        private readonly IndexerClient _indexerClient;

        public MainChainService(IndexerClient indexerClient)
        {
            _indexerClient = indexerClient;
        }

        public async Task<ConcurrentChain> GetMainChainAsync()
        {
            return _indexerClient.GetMainChain();
        }

        public async Task<ConcurrentChain> UpdateChain(ConcurrentChain source)
        {
            var chainChanges = _indexerClient.GetChainChangesUntilFork(source.Tip, false);

            chainChanges.UpdateChain(source);

            return source;
        }
    }
}
