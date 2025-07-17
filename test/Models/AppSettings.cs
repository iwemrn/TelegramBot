namespace test.Models
{
    public class AppSettings
    {
        public List<BotConfig> Bots { get; set; }
        public ServiceSettings Settings { get; set; }
    }

    public class ServiceSettings
    {
        public int MessagesPerMinute { get; set; }
        public int RetryCount { get; set; }
        public int RetryDelaySeconds { get; set; }
    }

}
