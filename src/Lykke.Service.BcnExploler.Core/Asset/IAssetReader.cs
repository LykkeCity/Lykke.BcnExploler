using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Contracts;

namespace Lykke.Service.BcnExploler.Core.Asset
{

    public interface IAssetReader
    {
        Task<IAssetDefinition> ReadAssetDataAsync(string absUrl);
    }
}
