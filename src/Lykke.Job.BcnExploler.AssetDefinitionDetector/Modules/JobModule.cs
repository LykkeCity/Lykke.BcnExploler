using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BcnExploler.Core.Health;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Services.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.BcnExploler.AssetDefinitionDetector.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<BcnExplolerSettings> _settingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<BcnExplolerSettings> settingsManager, ILog log)
        {
            _settingsManager = settingsManager;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            // NOTE: You can implement your own poison queue notifier. See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            // TODO: Add your dependencies here
            builder.RegisterInstance(_settingsManager.CurrentValue).As<BcnExplolerSettings>();
            builder.Populate(_services);
        }
    }
}