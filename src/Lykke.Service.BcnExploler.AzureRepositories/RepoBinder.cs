using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Core.Asset;
using Lykke.Service.BcnExploler.AzureRepositories.Asset;
using Lykke.Service.BcnExploler.Core.Asset;
using Lykke.Service.BcnExploler.Core.Settings;

namespace Lykke.Service.BcnExploler.AzureRepositories
{
    public static class RepoBinder
    {
        public static void BindAzureRepositories(this ContainerBuilder builder, 
            AppSettings generalSettings, 
            ILog log)
        {
            builder.Register(p =>
                new AssetDefinitionRepository(AzureTableStorage<AssetDefinitionDefinitionEntity>
                .Create(()=> generalSettings.BcnExplolerService.Db.AssetsConnString, 
                    "AssetDefinitions", 
                    log))).As<IAssetDefinitionRepository>();

            builder.Register(p =>
                new AssetDefinitionParsedBlockRepository(AzureTableStorage<AssetDefinitionParsedBlockEntity>
                    .Create(() => generalSettings.BcnExplolerService.Db.AssetsConnString,
                        "AssetParsedBlocks",
                        log))).As<IAssetDefinitionParsedBlockRepository>();


            builder.Register(p =>
                new AssetImageRepository(AzureTableStorage<AssetImageEntity>
                    .Create(() => generalSettings.BcnExplolerService.Db.AssetsConnString,
                        "AssetImages",
                        log))).As<IAssetImageRepository>();

        }
    }
}
