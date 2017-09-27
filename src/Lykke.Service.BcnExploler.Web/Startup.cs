using System;
using System.Reflection;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.JobTriggers.Triggers;
using Lykke.Logs;
using Lykke.Service.BcnExploler.Core.Settings;
using Lykke.Service.BcnExploler.Web.Filters;
using Lykke.Service.BcnExploler.Web.Jobs;
using Lykke.Service.BcnExploler.Web.Modules;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BcnExploler.Web
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; set; }
        public IConfigurationRoot Configuration { get; }
        public IServiceProvider ServiceProvider { get; set; }
        private TriggerHost TriggerHost { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;

            Console.WriteLine($"ENV_INFO: {System.Environment.GetEnvironmentVariable("ENV_INFO")}");
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
                {
                    o.Filters.Add(new HandleAllExceptionsFilterFactory());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "BcnExploler API");
            });

            var builder = new ContainerBuilder();
            var appSettings = Configuration.LoadSettings<AppSettings>();
            var log = CreateLogWithSlack(services, appSettings);

            builder.RegisterModule(new ServiceModule(appSettings, log));
            builder.Populate(services);
            ApplicationContainer = builder.Build();
            ServiceProvider = new AutofacServiceProvider(ApplicationContainer);
            
            return ServiceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseStaticFiles();

            this.TriggerHost = new TriggerHost(ServiceProvider);
            this.TriggerHost.ProvideAssembly(typeof(MainChainFunctions).GetTypeInfo().Assembly);

            appLifetime.ApplicationStarted.Register(StartApplication);
            appLifetime.ApplicationStopping.Register(StopApplication);
            appLifetime.ApplicationStopped.Register(CleanUp);
        }

        private void StartApplication()
        {
            new Thread(() =>
            {
                this.TriggerHost.Start().Wait();
            }).Start();
        }


        private void StopApplication()
        {
            this.TriggerHost.Cancel();
            // TODO: Implement your shutdown logic here. 
            // Service still can recieve and process requests here, so take care about it.
        }

        private void CleanUp()
        {
            // TODO: Implement your clean up logic here.
            // Service can't recieve and process requests here, so you can destroy all resources

            ApplicationContainer.Dispose();
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.BcnExploler.Db.LogsConnString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            // Creating azure storage logger, which logs own messages to concole log
            if (!string.IsNullOrEmpty(dbLogConnectionString) && !(dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}")))
            {
                const string appName = "Lykke.Job.BcnExploler.Web";

                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    appName,
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "BcnExploleWebLog", consoleLogger),
                    consoleLogger);

                var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(appName, slackService, consoleLogger);

                var azureStorageLogger = new LykkeLogToAzureStorage(
                    appName,
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);

                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }

            return aggregateLogger;
        }
    }
}
