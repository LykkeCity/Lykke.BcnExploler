using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Transaction;

namespace Lykke.Service.BcnExploler.Core.Channel
{
    public interface IFilledChannel:IChannel
    {
        ITransaction OpenFilledTransaction { get; }
        ITransaction CloseFilledTransaction { get; }
    }

    public interface IFilledMixedTransaction : IMixedChannelTransaction
    {
        ITransaction FilledOnchainTransactionData { get; }
    }
    

    public interface IOffchainNotificationsService
    {
        Task<IMixedChannelTransaction> GetOffchainMixedTransaction(string txId);
        Task<bool> OffchainTransactionExistsAsync(string transactionId);
        Task<long> GetTransactionCountByAddressAsync(string address);
        Task<long> GetTransactionCountByGroupAsync(string group);
        Task<IEnumerable<IChannel>> GetChannelsByAddressAsync(string address, ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All);
        Task<bool> IsHubAsync(string address);
        Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByAddressAsync(string address, IPageOptions pageOptions);



        Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByGroupAsync(string groupId, IPageOptions pageOptions);
    }
}
