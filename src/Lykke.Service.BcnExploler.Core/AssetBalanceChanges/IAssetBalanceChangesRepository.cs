using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.AssetBalanceChanges
{
    public interface IBalanceChanges
    {
        string AssetId { get; }
        long Quantity { get; }
        string BlockHash { get;  }
        int BlockHeight { get; }
        string TransactionHash { get;  }
    }

    public interface IBalanceSummary
    {
        IEnumerable<string> AssetIds { get; }
        IEnumerable<IBalanceAddressSummary> AddressSummaries { get;  }
    }

    public interface IBalanceAddressSummary
    {
        string Address { get; }
        double Balance { get; }
    }

    public interface IBalanceTransaction
    {
        string Hash { get; }
    }

    public interface IBalanceBlock
    {
        int Height { get; }
    }

    public interface IQueryOptions
    {
        int FromBlockHeight { get; }
        int ToBlockHeight { get; }
    }

    public class QueryOptions:IQueryOptions
    {
        public int FromBlockHeight { get; set; }
        public int ToBlockHeight { get; set; }

        public QueryOptions()
        {
            FromBlockHeight = 0;
            ToBlockHeight = int.MaxValue;
        }

        public static QueryOptions Create()
        {
            return new QueryOptions();
        }

        public QueryOptions From(int fromBlockHeight)
        {
            this.FromBlockHeight = fromBlockHeight;

            return this;
        }

        public QueryOptions To(int toBlockHeight)
        {
            this.ToBlockHeight = toBlockHeight;

            return this;
        }
    }

    public interface IAssetBalanceChangesRepository
    {
        Task<IBalanceSummary> GetSummaryAsync(params string[] assetIds);
        Task<IBalanceSummary> GetSummaryAsync(int? at, params string[] assetIds);
        Task<IEnumerable<IBalanceTransaction>> GetTransactionsAsync(IEnumerable<string> assetIds, int? fromBlock = null);
        Task<IBalanceTransaction> GetLatestTxAsync(IEnumerable<string> assetIds);

        /// <summary>
        /// Get Dictionary with address changes at specific block. Key - address, Value - quantity change
        /// </summary>      
        Task<IDictionary<string, double>> GetAddressQuantityChangesAtBlock(int blockHeight, IEnumerable<string> assetIds);

        /// <summary>
        /// Get blocks where asset change happens
        /// </summary>
        Task<IEnumerable<IBalanceBlock>> GetBlocksWithChanges(IEnumerable<string> assetIds);
    }
}
