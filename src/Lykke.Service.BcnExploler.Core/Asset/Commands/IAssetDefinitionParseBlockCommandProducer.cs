using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Commands
{
    public class AssetDefinitionParseBlockContext
    {
        public string BlockHash { get; set; }
    }

    public interface IAssetDefinitionParseBlockCommandProducer
    {
        Task CreateParseBlockCommand(string blockHash);
    }
}
