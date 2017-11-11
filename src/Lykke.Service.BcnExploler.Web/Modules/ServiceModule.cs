using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.JobTriggers.Extenstions;
using Lykke.Service.BcnExploler.AzureRepositories;
using Lykke.Service.BcnExploler.Core;
using Lykke.Service.BcnExploler.Core.Health;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services;
using Lykke.Service.BcnExploler.Services.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BcnExploler.Web.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings.CurrentValue);
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

	        builder.RegisterInstance(new ConsoleLWriter(Console.WriteLine)).As<IConsole>();

			builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
            
            builder.BindAzureRepositories(_settings, _log);
            builder.BindCommonServices(_settings, _log);
            builder.AddTriggers(pool =>
            {
                // default connection must be initialized
                pool.AddDefaultConnection(_settings.CurrentValue.BcnExploler.Db.AssetsConnString);
            });

            builder.Populate(_services);
        }
    }
}
