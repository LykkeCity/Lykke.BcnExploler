using Autofac;
using Common.Log;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Domain.Block;
using Lykke.Service.BcnExploler.Core.Services;
using Lykke.Service.BcnExploler.Services.Domain;

namespace Lykke.Service.BcnExploler.Services
{
    public static class SrvBinder
    {
        public static void BindCommonServices(this ContainerBuilder builder, AppSettings generalSettings, ILog log)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<BlockService>()
                .As<IBlockService>()
                .SingleInstance();
        }
    }
}
