﻿using System.Collections.Generic;
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
    

    public interface IChannelService
    {
        Task<IFilledChannel> GetChannelsByOffchainTransactionIdAsync(string transactionId);
        Task<bool> OffchainTransactionExistsAsync(string transactionId);

        Task<IEnumerable<IFilledChannel>> GetByBlockAsync(string blockId,
            ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All,
            IPageOptions pageOptions = null);

        Task<long> GetChannelCountByBlockAsync(string blockId);
        Task<IEnumerable<IFilledChannel>> GetChannelsByAddressFilledAsync(string address, 
            ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All,
            IPageOptions pageOptions = null);
        
        Task<long> GetTrabsactionCountByAddressAsync(string address);
        Task<IEnumerable<IChannel>> GetChannelsByAddressAsync(string address, ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All);
        Task<bool> IsHubAsync(string address);
        Task<IEnumerable<IFilledMixedTransaction>> GetMixedTransactionsByAddressAsync(string address, IPageOptions pageOptions);
    }
}
