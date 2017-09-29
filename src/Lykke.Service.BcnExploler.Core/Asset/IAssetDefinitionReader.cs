using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Contracts;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;

namespace Lykke.Service.BcnExploler.Core.Asset
{

    public interface IAssetDefinitionReader
    {
        Task<IAssetDefinition> ReadAssetDataAsync(string absUrl);
    }
}
