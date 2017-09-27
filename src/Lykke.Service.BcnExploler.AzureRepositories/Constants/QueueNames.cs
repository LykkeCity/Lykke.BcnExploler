using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BcnExploler.AzureRepositories.Constants
{
    public static class QueueNames
    {
        public static class AssetDefinitionScanner
        {

            public const string RetrieveAsset = "asset-definitions-retrieve-data";
            public const string ParseBlock = "asset-definitions-parse-block";
            public const string UpsertAssetImages = "asset-definitions-upsert-images";
        }
    }
}
