﻿using Autofac;
using AzureStorage.Blob;
using Common;
using Common.Cache;
using Common.Log;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Address;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Health;
using Lykke.Service.BcnExploler.Core.MainChain;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Services.Address;
using Lykke.Service.BcnExploler.Services.Asset;
using Lykke.Service.BcnExploler.Services.AssetBalanceChanges;
using Lykke.Service.BcnExploler.Services.Health;
using Lykke.Service.BcnExploler.Services.Ninja;
using Lykke.Service.BcnExploler.Services.Ninja.Address;
using Lykke.Service.BcnExploler.Services.Ninja.Block;
using Lykke.Service.BcnExploler.Services.Ninja.MainChain;
using Lykke.Service.BcnExploler.Services.Ninja.Transaction;
using Lykke.Service.BcnExploler.Services.Settings;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Lykke.Service.BcnExploler.Services
{
    public static class SrvBinder
    {
        public static void BindCommonServices(this ContainerBuilder builder, AppSettings generalSettings, ILog log)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            var indexerConfiguration = new IndexerConfiguration
            {
                StorageCredentials = new StorageCredentials(generalSettings.BcnExplolerService.NinjaIndexerCredentials.AzureName, 
                generalSettings.BcnExplolerService.NinjaIndexerCredentials.AzureKey),
                Network = generalSettings.BcnExplolerService.UsedNetwork()
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

            builder.RegisterType<MainChainService>()
                .As<IMainChainService>()
                .InstancePerDependency();

            var cachedMainChainConnString = generalSettings.BcnExplolerService.Db.AssetsConnString;

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
        }
    }
}
