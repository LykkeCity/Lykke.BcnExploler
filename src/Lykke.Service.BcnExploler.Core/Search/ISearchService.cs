using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Search
{
    public enum SearchResultType
    {
        Block,
        Transaction,
        Address,
        Asset
    }

    public interface ISearchService
    {
        Task<SearchResultType?> GetTypeAsync(string id);
    }
}
