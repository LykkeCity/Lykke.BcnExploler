using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Contracts;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;

namespace Lykke.Service.BcnExploler.Services.Asset
{
    public class AssetDefinition : IAssetDefinition
    {
        public IEnumerable<string> AssetIds { get; set; }

        public string ContactUrl { get; set; }

        public string NameShort { get; set; }

        public string Name { get; set; }

        public string Issuer { get; set; }

        public string Description { get; set; }

        public string DescriptionMime { get; set; }

        public string Type { get; set; }

        public int Divisibility { get; set; }

        public bool LinkToWebsite { get; set; }

        public string IconUrl { get; set; }

        public string ImageUrl { get; set; }

        public string Version { get; set; }
        public string AssetDefinitionUrl { get; set; }

        public static AssetDefinition Create(AssetDefinitionContract source)
        {
            return new AssetDefinition
            {
                AssetIds = source.AssetIds ?? Enumerable.Empty<string>(),
                ContactUrl = source.ContactUrl,
                Description = source.Description,
                DescriptionMime = source.DescriptionMime,
                Divisibility = source.Divisibility,
                IconUrl = source.IconUrl,
                ImageUrl = source.ImageUrl,
                Issuer = source.Issuer,
                LinkToWebsite = source.LinkToWebsite,
                Name = source.Name,
                NameShort = source.NameShort,
                Type = source.Type,
                Version = source.Version,
                AssetDefinitionUrl = source.AssetDefinitionUrl
            };
        }
    }
    public class AssetDefinitionReader:IAssetReader
    {
        public async Task<IAssetDefinition> ReadAssetDataAsync(string absUrl)
        {
            try
            {
                var resp = await absUrl.GetJsonAsync<AssetDefinitionContract>();

                return AssetDefinition.Create(resp);
            }
            catch
            {
                return null;
            }
        }
    }
}
