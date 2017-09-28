using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Search
{
    public enum SearchResultType
    {
        Block,
        Transaction,
        Address,
        Asset,
        OffchainTransaction,
    }

    public interface ISearchService
    {
        Task<SearchResultType?> GetTypeAsync(string id);
    }
}
