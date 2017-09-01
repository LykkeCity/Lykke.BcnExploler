using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Core.MainChain
{
    public interface ICachedMainChainService
    {
        Task UpdatePersistentCacheAsync();
        Task UpdateTemporaryCacheAsync();
        Task<ConcurrentChain> GetMainChainAsync();
    }
}
