using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Definitions.Commands
{
    public class UpdateAssetDataContext
    {
        public string AssetDefinitionUrl { get; set; }
    }

    public interface IAssetDefinitionCommandProducer
    {
        Task CreateRetrieveAssetDefinitionCommand(params string[] urls);
    }
}
