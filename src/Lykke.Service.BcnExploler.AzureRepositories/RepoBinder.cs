using System;
using Autofac;
using AzureStorage.Queue;
using AzureStorage.Tables;
using Common.Log;
using Core.Asset;
using Lykke.Service.BcnExploler.AzureRepositories.Asset;
using Lykke.Service.BcnExploler.AzureRepositories.Asset.Commands;
using Lykke.Service.BcnExploler.AzureRepositories.Constants;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Commands;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.BcnExploler.AzureRepositories
{
    public static class RepoBinder
    {
        public static void BindAzureRepositories(this ContainerBuilder builder, 
            IReloadingManager<AppSettings> generalSettings, 
            ILog log)
        {
            builder.RegisterRepo(generalSettings, log);
            builder.RegisterQueues(generalSettings, log);
        }

        private static void RegisterRepo(this ContainerBuilder builder,
            IReloadingManager<AppSettings> generalSettings,
            ILog log)
        {
            builder.Register(p =>
                new AssetDefinitionRepository(AzureTableStorage<AssetDefinitionDefinitionEntity>
                    .Create(generalSettings.ConnectionString(x=>x.BcnExploler.Db.AssetsConnString),
                        "AssetDefinitions",
                        log))).As<IAssetDefinitionRepository>();

            builder.Register(p =>
                new AssetDefinitionParsedBlockRepository(AzureTableStorage<AssetDefinitionParsedBlockEntity>
                    .Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString),
                        "AssetParsedBlocks",
                        log))).As<IAssetDefinitionParsedBlockRepository>();


            builder.Register(p =>
                new AssetImageRepository(AzureTableStorage<AssetImageEntity>
                    .Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString),
                        "AssetImages",
                        log))).As<IAssetImageRepository>();

            builder.Register(p =>
                new AssetScoreRepository(AzureTableStorage<AssetScoreEntity>
                    .Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString),
                        "AssetScores",
                        log))).As<IAssetScoreRepository>();

            builder.Register(p =>
                new AssetCoinholdersIndexRepository(AzureTableStorage<AssetCoinholdersIndexEntity>
                    .Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString),
                        "AssetCoinholdersIndexes",
                        log))).As<IAssetCoinholdersIndexRepository>();

        }

        private static void RegisterQueues(this ContainerBuilder builder,
            IReloadingManager<AppSettings> generalSettings,
            ILog log)
        {
            builder.RegisterInstance(new AssetDefinitionParseBlockCommandProducer(AzureQueueExt.Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString), QueueNames.AssetDefinitionScanner.ParseBlock)))
                .As<IAssetDefinitionParseBlockCommandProducer>();


            builder.RegisterInstance(new AssetDataCommandProducer(AzureQueueExt.Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString), QueueNames.AssetDefinitionScanner.RetrieveAsset)))
                .As<IAssetDataCommandProducer>();

            builder.RegisterInstance(new AssetImageCommandProducer(AzureQueueExt.Create(generalSettings.ConnectionString(x => x.BcnExploler.Db.AssetsConnString), QueueNames.AssetDefinitionScanner.UpsertAssetImages)))
                .As<IAssetImageCommandProducer>();
        }
    }
}
