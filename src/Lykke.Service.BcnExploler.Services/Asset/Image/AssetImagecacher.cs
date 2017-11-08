using System;
using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Images;

namespace Lykke.Service.BcnExploler.Services.Asset.Image
{

    public class ImageSaveResult : IImageSaveResult
    {
        public bool Saved { get; set; }
        public string CachedUrl { get; set; }

        public static ImageSaveResult Ok(string url)
        {
            return new ImageSaveResult
            {
                Saved = true,
                CachedUrl = url
            };
        }

        public static ImageSaveResult Fail()
        {
            return new ImageSaveResult
            {
                Saved = false
            };
        }
    }

    public class AssetImageCacher : IAssetImageCacher
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ILog _log;

        private const string IconContainer = "icons";
        private const string ImageContainer = "images";

        public AssetImageCacher(IBlobStorage blobStorage, ILog log)
        {
            _blobStorage = blobStorage;
            _log = log;
        }

        public async Task<IImageSaveResult> SaveAssetIconAsync(string url, string assetId)
        {
            return await Save(url, IconContainer, assetId);
        }

        public async Task<IImageSaveResult> SaveAssetImageAsync(string url, string assetId)
        {
            return await Save(url, ImageContainer, assetId);
        }

        private async Task<IImageSaveResult> Save(string url, string container, string assetId)
        {
            if (string.IsNullOrEmpty(url))
            {
                return ImageSaveResult.Fail();
            }
            try
            {
                var key = GenerateKeyName(assetId, GetImageExtension(url));

                var resp = await url.GetAsync();

                var savedUrl = await _blobStorage.SaveBlobAsync(container, key, await resp.Content.ReadAsStreamAsync());

                return ImageSaveResult.Ok(savedUrl);
            }
            catch (FlurlHttpException)
            {
	            return ImageSaveResult.Fail();
            }
			catch (Exception e)
            {
                await _log.WriteInfoAsync("AssetImageCacher", "Save", url, e.ToString());

                return ImageSaveResult.Fail();
            }
        }

        private string GetImageExtension(string url)
        {
            var uri = new Uri(url);
            return Path.GetExtension(uri.AbsolutePath);
        }

        private string GenerateKeyName(string assetId, string extension)
        {
            return assetId + extension;
        }
    }
}
