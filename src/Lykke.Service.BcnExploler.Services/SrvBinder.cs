using Autofac;
using Autofac.Builder;
using Common.Log;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Domain.Block;
using Lykke.Service.BcnExploler.Core.Services;
using Lykke.Service.BcnExploler.Services.Domain;
using Lykke.Service.BcnExploler.Services.Domain.Settings;
using Microsoft.WindowsAzure.Storage.Auth;
using NBitcoin;
using NBitcoin.Indexer;

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
            builder.Register(p => new IndexerClient(indexerConfiguration) {ColoredBalance = true}).AsSelf().InstancePerDependency();

            builder.RegisterType<BlockService>()
                .As<IBlockService>()
                .InstancePerDependency();
        }
    }
}
