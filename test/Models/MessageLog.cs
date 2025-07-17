namespace test.Models
{
    public class MessageLog
    {
        public int id { get; set; }
        public string BotName { get; set; }
        public string ChatId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; internal set; }
        public bool IsSuccess { get; set; }
    }

}
