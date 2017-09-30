using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.BcnExploler.Core.Asset.Definitions
{
    public interface IAssetDefinition
    {
        IEnumerable<string> AssetIds { get; }

        string ContactUrl { get; }

        string NameShort { get; }

        string Name { get; }

        string Issuer { get; }

        string Description { get; }

        string DescriptionMime { get; }

        string Type { get; }

        int Divisibility { get; }

        bool LinkToWebsite { get; }

        string IconUrl { get; set; }

        string ImageUrl { get; set; }

        string Version { get; }
        string AssetDefinitionUrl { get; }
    }

    public static class AssetHelper
    {
        public static bool IsVerified(this IAssetDefinition assetDefinition)
        {
            var url = assetDefinition.AssetDefinitionUrl ?? "";
            Uri uriResult;
            var isHttps = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            var isCoinPrismDomain = url.Contains("cpr.sm");

            return isHttps && !isCoinPrismDomain;
        }

        public static string IssuerWebsite(this IAssetDefinition assetDefinition)
        {
            var url = assetDefinition.ContactUrl ?? "";

            Uri uriResult;
            var isCorrectUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult);

            if (isCorrectUrl)
            {
                return uriResult.Scheme + Uri.SchemeDelimiter + uriResult.Host;
            }

            return null;
        }

        public static bool IsValid(this IAssetDefinition assetDefinition)
        {
            return assetDefinition.AssetIds.Any() && assetDefinition.AssetIds.All(x => x != null);
        }
    }
}
