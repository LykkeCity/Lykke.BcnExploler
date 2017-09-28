using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;

namespace Lykke.Service.BcnExploler.Core.Asset
{
    public interface IAssetService
    {
        Task<IAssetDefinition> GetAssetAsync(string assetId);
        Task<IAssetDefinition> GetAssetDefinitionByDefUrlAsync(string url);
        Task<IReadOnlyDictionary<string, IAssetDefinition>> GetAssetDefinitionDictionaryAsync();
        Task<IEnumerable<IAssetDefinition>> GetAssetDefinitionsAsync();
        Task<IEnumerable<IAssetDefinition>> GetAssetDefinitionsAsync(string issuer);
        Task<IDictionary<string, IAssetCoinholdersIndex>> GetAssetCoinholdersIndexAsync();
        Task<IDictionary<string, IAssetScore>> GetAssetScoreDictionaryAsync();
    }
}
