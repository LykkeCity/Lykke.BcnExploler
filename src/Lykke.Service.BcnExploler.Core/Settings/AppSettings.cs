namespace Lykke.Service.BcnExploler.Core.Settings
{
    public class AppSettings
    {
        public BcnExplolerSettings BcnExploler { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class BcnExplolerSettings
    {
        public DbSettings Db { get; set; }
        public string AssetStatsServiceUrl { get; set; }
        public string NinjaUrl { get; set; }
        public string Network { get; set; }

        public NinjaIndexerCredentials NinjaIndexerCredentials { get; set; }
    }

    public class NinjaIndexerCredentials
    {
        public string AzureName { get; set; }


        public string AzureKey { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string AssetsConnString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
