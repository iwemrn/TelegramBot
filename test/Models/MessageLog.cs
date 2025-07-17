namespace test.Models
{
    public class MessageLog
    {
        public int Id { get; set; }
        public string BotName { get; set; }
        public string ChatId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
    }

}
