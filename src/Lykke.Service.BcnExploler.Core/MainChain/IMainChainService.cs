using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BcnExploler.Core.MainChain
{
    public interface IMainChainService
    {
        Task<ConcurrentChain> GetMainChainAsync();
        Task<ConcurrentChain> UpdateChain(ConcurrentChain source);
    }
}
