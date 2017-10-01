using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Flurl.Http;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Contracts;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Newtonsoft.Json;

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

        public static AssetDefinition Create(AssetDefinitionContract source, string assetDefinitionUrl)
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
                AssetDefinitionUrl = assetDefinitionUrl
            };
        }
    }
    public class AssetDefinitionReader:IAssetDefinitionReader
    {

        public async Task<IAssetDefinition> ReadAssetDataAsync(string absUrl)
        {
            try
            {
                var respString = await GetIgnoreCertErrorAsync(absUrl);
                var resp = JsonConvert.DeserializeObject<AssetDefinitionContract>(respString);

                return AssetDefinition.Create(resp, absUrl);
            }
            catch (Exception)
            {
               
                return null;
            }
        }

        private async Task<string> GetIgnoreCertErrorAsync(string absUrl)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;

                using (var client = new HttpClient(httpClientHandler))
                {
                    return await client.GetStringAsync(absUrl);
                }
            }
        }
    }
}
