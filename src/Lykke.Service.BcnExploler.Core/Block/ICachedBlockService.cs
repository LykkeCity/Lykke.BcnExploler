using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Block
{
    public interface ICachedBlockService
    {
        Task<IBlock> GetBlockAsync(string id);
    }
}
