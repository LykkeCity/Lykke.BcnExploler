using System.Threading.Tasks;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;

namespace Lykke.Service.BcnExploler.Core.Asset.Indexes.Commands
{
    public class AssetCoinholdersUpdateIndexCommand
    {
        public string AssetId { get; set; }
    }

    public interface IAssetCoinholdersIndexesCommandProducer
    {
        Task CreateAssetCoinholdersUpdateIndexCommand(params IAssetDefinition[] assetsDefinition);
        Task CreateAssetCoinholdersUpdateIndexCommand(params string[] assetIds);
    }
}
