using System;
using System.Linq;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Services.Helpers
{
    public static class ConcurrentChainHelper
    {
        public static ChainedBlock GetClosestToTimeBlock(this ConcurrentChain chain, DateTime utcTime)
        {
            return chain.ToEnumerable(true).FirstOrDefault(p => p.Header.BlockTime.UtcDateTime <= utcTime);
        }
    }
}
