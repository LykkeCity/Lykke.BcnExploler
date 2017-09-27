using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Definitions
{
    public interface IAssetDefinitionParsedBlock
    {
        string BlockHash { get; }
    }

    public class AssetDefinitionParsedBlock: IAssetDefinitionParsedBlock
    {
        public string BlockHash { get; set; }

        public static AssetDefinitionParsedBlock Create(string hash)
        {
            return new AssetDefinitionParsedBlock
            {
                BlockHash = hash
            };
        }
    }

    public interface IAssetDefinitionParsedBlockRepository
    {
        Task<bool> IsBlockExistsAsync(IAssetDefinitionParsedBlock block);
        Task AddBlockAsync(IAssetDefinitionParsedBlock block);
    }
}
