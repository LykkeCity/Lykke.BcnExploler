using System.Threading.Tasks;

namespace Lykke.Service.BcnExploler.Core.Asset.Definitions.Images
{
    public interface IAssetImageCacher
    {
        Task<IImageSaveResult> SaveAssetIconAsync(string url, string assetId);
        Task<IImageSaveResult> SaveAssetImageAsync(string url, string assetId);
    }

    public interface IImageSaveResult
    {
        bool Saved { get; }

        string CachedUrl { get; }
    }
}
