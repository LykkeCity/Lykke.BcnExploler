using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Transaction;

namespace Lykke.Service.BcnExploler.Core.OffchainNotifcations
{
    public interface IFilledMixedTransaction : IMixedTransaction
    {
        ITransaction FilledOnchainTransactionData { get; }
    }
    
    public interface IOffchainNotificationsService
    {
        Task<IMixedTransaction> GetOffchainMixedTransaction(string txId);
        Task<bool> OffchainTransactionExistsAsync(string transactionId);
        Task<long> GetTransactionCountByAddressAsync(string address);
        Task<long> GetTransactionCountByGroupAsync(string group);
        Task<bool> IsHubAsync(string address);
        Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByAddressAsync(string address, IPageOptions pageOptions);
        Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByGroupAsync(string groupId, IPageOptions pageOptions);
        Task<IEnumerable<IGroup>> GetGroups(string address, bool openOnly = true, int take = 10,
            bool offchainOnly = true);
    }
}
