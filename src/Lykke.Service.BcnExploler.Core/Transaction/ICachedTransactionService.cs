using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Transaction;

namespace Lykke.Service.BcnExploler.Core.Transaction
{
    public interface ICachedTransactionService
    {
        Task<ITransaction> GetAsync(string id);
        Task<IEnumerable<ITransaction>> GetAsync(IEnumerable<string> ids);
    }
}
