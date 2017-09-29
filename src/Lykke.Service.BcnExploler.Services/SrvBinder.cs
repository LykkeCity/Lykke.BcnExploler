using Autofac;
using AzureStorage.Blob;
using Common;
using Common.Cache;
using Common.Log;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Asset.Definitions;
using Lykke.Service.BcnExploler.Core.Asset.Definitions.Images;
using Lykke.Service.BcnExploler.Core.Asset.Indexes;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Channel;
using Lykke.Service.BcnExploler.Core.Health;
using Lykke.Service.BcnExploler.Core.MainChain;
using Lykke.Service.BcnExploler.Core.Search;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Services.Address;
using Lykke.Service.BcnExploler.Services.Asset;
using Lykke.Service.BcnExploler.Services.Asset.Image;
using Lykke.Service.BcnExploler.Services.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Services.Channel;
using Lykke.Service.BcnExploler.Services.Health;
using Lykke.Service.BcnExploler.Services.Ninja;
using Lykke.Service.BcnExploler.Services.Ninja.Address;
using Lykke.Service.BcnExploler.Services.Ninja.Block;
using Lykke.Service.BcnExploler.Services.Ninja.MainChain;
using Lykke.Service.BcnExploler.Services.Ninja.Transaction;
using Lykke.Service.BcnExploler.Services.Search;
using Lykke.Service.BcnExploler.Services.Settings;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Lykke.Service.BcnExploler.Services
{
    public static class SrvBinder
    {
        public static void BindCommonServices(this ContainerBuilder builder, IReloadingManager<AppSettings> generalSettingsManager, ILog log)
        {
            var generalSettings = generalSettingsManager.CurrentValue;
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            var indexerConfiguration = new IndexerConfiguration
            {
                StorageCredentials = new StorageCredentials(generalSettings.BcnExploler.NinjaIndexerCredentials.AzureName, 
                generalSettings.BcnExploler.NinjaIndexerCredentials.AzureKey),
                Network = generalSettings.BcnExploler.UsedNetwork()
            };
            builder.Register(p => new IndexerClient(indexerConfiguration)).AsSelf().InstancePerDependency();

            builder.RegisterType<AssetBalanceChangesRepository>()
                .As<IAssetBalanceChangesRepository>()
                .InstancePerDependency();

            builder.RegisterType<BlockService>()
                .As<IBlockService>()
                .InstancePerDependency();

            builder.RegisterType<AddressService>()
                .As<IAddressService>()
                .InstancePerDependency();

            builder.RegisterType<TransactionService>()
                .As<ITransactionService>()
                .InstancePerDependency();

            builder.RegisterType<MainChainService>()
                .As<IMainChainService>()
                .InstancePerDependency();

            builder.RegisterType<SearchService>()
                .As<ISearchService>()
                .InstancePerDependency();

            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return
                        new CachedDataDictionary<string, IAssetDefinition>(
                            async () => AssetIndexer.IndexAssetsDefinitions(
                                await context.Resolve<IAssetDefinitionRepository>().GetAllAsync(),
                                await context.Resolve<IAssetImageRepository>().GetAllAsync())
                            , validDataInSeconds: 1 * 10 * 60);
                }
            ).AsSelf().SingleInstance();


            builder.RegisterInstance(new AssetImageCacher(
                AzureBlobStorage.Create(
                    generalSettingsManager.ConnectionString(p => p.BcnExploler.Db.AssetsConnString)), log))
                    .As<IAssetImageCacher>();

            builder.RegisterInstance(generalSettingsManager.CurrentValue.BcnExploler.UsedNetwork())
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MainChainService>()
                .As<IMainChainService>()
                .InstancePerDependency();

            var cachedMainChainConnString = generalSettings.BcnExploler.Db.AssetsConnString;

#if DEBUG
            cachedMainChainConnString = "UseDevelopmentStorage=true";
#endif
            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return
                        new CachedMainChainService(new MemoryCacheManager(), new AzureBlobStorage(cachedMainChainConnString), context.Resolve<IMainChainService>(), context.Resolve<AppSettings>() );
                }
            ).As<ICachedMainChainService>()
            .SingleInstance();

            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return
                        new CachedDataDictionary<string, IAssetCoinholdersIndex>(
                            async () => AssetIndexer.IndexAssetCoinholders(await context.Resolve<IAssetCoinholdersIndexRepository>().GetAllAsync())
                            , validDataInSeconds: 1 * 10 * 60);
                }
            ).AsSelf().SingleInstance();

            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return
                        new CachedDataDictionary<string, IAssetScore>(
                            async () => AssetIndexer.IndexAssetScores(await context.Resolve<IAssetScoreRepository>().GetAllAsync())
                            , validDataInSeconds: 1 * 10 * 60);
                }
            ).AsSelf().SingleInstance();

            builder.RegisterType<AssetDefinitionReader>().As<IAssetDefinitionReader>();

            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return new CachedBlockService(new MemoryCacheManager(), context.Resolve<IBlockService>());
                }
            ).As<ICachedBlockService>().SingleInstance();


            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return new CachedTransactionService(new MemoryCacheManager(), context.Resolve<ITransactionService>());
                }
            ).As<ICachedTransactionService>().SingleInstance();

            builder.Register(p =>
                {
                    var context = p.Resolve<IComponentContext>();
                    return new CachedAddressService(new MemoryCacheManager(), context.Resolve<IAddressService>());
                }
            ).As<ICachedAddressService>().SingleInstance();

            builder.RegisterType<AssetService>()
                .As<IAssetService>()
                .SingleInstance();

            builder.RegisterType<OffchainNotificationsApiProvider>()
                .As<IOffchainNotificationsApiProvider>()
                .SingleInstance();

            builder.RegisterType<ChannelService>()
                .As<IChannelService>()
                .SingleInstance();
        }
    }
}
