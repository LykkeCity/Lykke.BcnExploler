using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Commands
{
    public class AssetImageContext
    {
        public IEnumerable<string> AssetIds { get; set; }

        public string IconUrl { get; set; }

        public string ImageUrl { get; set; }
    }

    public interface IAssetImageCommandProducer
    {
        Task CreateUpsertAssetImageCommand(IEnumerable<string> assetIds, string iconUrl, string imageUrl);
    }
}
