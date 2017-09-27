using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;

namespace Lykke.Service.BcnExploler.Core.Asset
{
    public interface IAssetDefinitionRepository
    {
        Task<IEnumerable<IAssetDefinition>> GetAllAsync();
        Task<IEnumerable<IAssetDefinition>> GetAllEmptyAsync();
        Task InsertOrReplaceAsync(params IAssetDefinition[] assetsDefinition);
        Task InsertEmptyAsync(string defUrl);
        Task<bool> IsAssetExistsAsync(string defUrl);
        Task RemoveEmptyAsync(params string[] defUrls);
        Task UpdateAssetAsync(IAssetDefinition assetDefinition);
    }
}
