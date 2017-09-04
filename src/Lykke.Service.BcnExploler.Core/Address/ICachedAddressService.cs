using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Address
{
    public interface ICachedAddressService
    {
        Task<IAddressTransactions> GetTransactions(string id);
    }
}
