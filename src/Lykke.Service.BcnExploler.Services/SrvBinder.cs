using Autofac;
using Common;
using Common.Cache;
using Common.Log;
using Core.Transaction;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Block;
using Lykke.Service.BcnExploler.Core.Health;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Core.Transaction;
using Lykke.Service.BcnExploler.Services.Asset;
using Lykke.Service.BcnExploler.Services.Health;
using Lykke.Service.BcnExploler.Services.Ninja;
using Lykke.Service.BcnExploler.Services.Ninja.Block;
using Lykke.Service.BcnExploler.Services.Ninja.Transaction;
using Lykke.Service.BcnExploler.Services.Settings;
using Microsoft.WindowsAzure.Storage.Auth;
using NBitcoin;

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

            builder.RegisterType<BlockService>()
                .As<IBlockService>()
                .InstancePerDependency();

            builder.RegisterType<TransactionService>()
                .As<ITransactionService>()
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

            builder.RegisterType<AssetService>()
                .As<IAssetService>()
                .SingleInstance();
        }
    }
}
