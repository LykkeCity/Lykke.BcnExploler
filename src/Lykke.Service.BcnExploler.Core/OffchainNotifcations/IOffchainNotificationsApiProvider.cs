using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.OffchainNotifcations
{
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
        decimal Address1QuantityDiff { get; }
        decimal Address2QuantityDiff { get; }
        MixedTransactionType Type { get; }
    }

    public interface IMixedTransaction
    {
        string AssetId { get; }

        string GroupId { get; }
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
        RevokedOffchain = 0,
        ConfirmedOffchain,
        ReOpenedOffchain,
        OpenOnChain,
        CloseOnchain,
        ReOpenOnChain,
        None
    }

    public interface IOnchainTransaction
    {
        string TransactionId { get; }
        MixedTransactionType Type { get; }
    }
    
    public interface IPageOptions
    {
        int ItemsToSkip { get; }
        int ItemsToTake { get; }
    }

    public interface IGroup
    {
        string GroupId { get;  }

        string AssetId { get;  }

        bool IsColored { get;  }

        string HubAddress { get;  }

        string Address1 { get;  }

        string Address2 { get;  }

        IEnumerable<IMixedTransaction> Transactions { get; }
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
        Task<IMixedTransaction> GetOffchainTransaction(string txId);
        Task<bool> OffchainTransactionExistsAsync(string transactionId);
        Task<bool> IsHubAsync(string address);

        Task<IEnumerable<IMixedTransaction>> GetMixedTransactionsByAddress(string address, IPageOptions pageOptions);
        Task<long> GetMixedTransactionCountByAddress(string address);
        Task<IEnumerable<IMixedTransaction>> GetMixedTransactionsByGroup(string group, IPageOptions pageOptions);
        Task<long> GetMixedTransactionCountByGroup(string group);

        Task<IEnumerable<IGroup>> GetGroups(string address, bool openOnly = true, int take = 10,
            bool offchainOnly = true);

    }
}
