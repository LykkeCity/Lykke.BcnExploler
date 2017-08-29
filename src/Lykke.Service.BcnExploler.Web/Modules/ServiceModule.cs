using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Services;
using Lykke.Service.BcnExploler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BcnExploler.Web.Modules
{
    public class ServiceModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.BindCommonServices(_settings, _log);

            builder.Populate(_services);
        }
    }
}
