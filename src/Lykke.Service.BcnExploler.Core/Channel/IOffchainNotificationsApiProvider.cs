﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Channel
{
    public interface IChannel
    {
        string AssetId { get; }
        bool IsColored { get; }
        IOnchainTransaction OpenTransaction { get; }

        IOnchainTransaction CloseTransaction { get; }

        IEnumerable<IOffchainTransaction> OffchainTransactions { get; }
    }

    public interface IOffchainTransaction
    {
        string TransactionId { get; }

        DateTime DateTime { get; }

        string HubAddress { get; }

        string Address1 { get; }

        string Address2 { get; }

        string AssetId { get; }

        bool IsColored { get; }

        decimal Address1Quantity { get; }

        decimal Address2Quantity { get; }

        bool IsRevoked { get; }
        decimal Address1QuantityDiff { get; }
        decimal Address2QuantityDiff { get; }
    }

    public interface IMixedChannelTransaction
    {
        string AssetId { get; }

        bool IsColored { get; }

        string HubAddress { get; }

        string ClientAddress1 { get; }

        string ClientAddress2 { get; }
        
        bool IsOffchain { get; }

        IOnchainTransaction OnchainTransactionData { get; }

        IOffchainTransaction OffchainTransactionData { get; }
    }

    public enum MixedTransactionType
    {
        Offchain,
        ChannelSetup,
        ReopenChannel,
        CloseChannel,
        None
    }

    public interface IOnchainTransaction
    {
        string TransactionId { get; }
        MixedTransactionType Type { get; }
    }

    public enum ChannelStatusQueryType
    {
        All,
        OpenOnly,
        ClosedOnly
    }

    public interface IPageOptions
    {
        int ItemsToSkip { get; }
        int ItemsToTake { get; }
    }

    public class PageOptions : IPageOptions
    {
        private int PageSize { get; set; }
        private int PageNumber { get; set; }

        public int ItemsToSkip => (PageNumber - 1) * PageSize;
        public int ItemsToTake => PageSize;

        public static PageOptions Create(int pageNumber, int pageSize)
        {
            return new PageOptions
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }

    public interface IOffchainNotificationsApiProvider
    {
        Task<IChannel> GetByOffchainTransactionIdAsync(string transactionId);
        Task<bool> OffchainTransactionExistsAsync(string transactionId);
        Task<IEnumerable<IChannel>> GetByAddressAsync(string address, ChannelStatusQueryType channelStatusQueryType = ChannelStatusQueryType.All, IPageOptions pageOptions = null);

        Task<bool> IsHubAsync(string address);
        Task<long> GetCountByAddressAsync(string address);

        Task<IEnumerable<IMixedChannelTransaction>> GetMixedTransactions(string address, IPageOptions pageOptions);
        Task<long> TransactionCountByAddress(string address);
    }
}
