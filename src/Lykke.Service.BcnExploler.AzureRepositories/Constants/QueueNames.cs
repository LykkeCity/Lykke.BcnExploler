namespace Lykke.Service.BcnExploler.AzureRepositories.Constants
{
    public static class QueueNames
    {
        public static class AssetDefinitionScanner
        {

            public const string RetrieveAsset = "asset-definitions-retrieve-data";
            public const string ParseBlock = "asset-definitions-parse-block";
            public const string UpsertImages = "asset-definitions-upsert-images";
        }

        public static class AssetIndexer
        {

            public const string UpdateIndex = "asset-indexer-update-index";
        }
    }
}
