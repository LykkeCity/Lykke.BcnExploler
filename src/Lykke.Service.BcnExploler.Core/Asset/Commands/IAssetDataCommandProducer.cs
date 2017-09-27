using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Commands
{
    public class UpdateAssetDataContext
    {
        public string AssetDefinitionUrl { get; set; }
    }

    public interface IAssetDataCommandProducer
    {
        Task CreateUpdateAssetDataCommand(params string[] urls);
    }
}
