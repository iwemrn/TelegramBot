using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using test.Models;

public class MessageSenderService
{
    private readonly AppDbContext _db;
    private readonly ILogger<MessageSenderService> _logger;
    private readonly AppSettings _config;

    public MessageSenderService(AppDbContext db, ILogger<MessageSenderService> logger, IOptions<AppSettings> config)
    {
        _db = db;
        _logger = logger;
        _config = config.Value;
    }

    public async Task SendMessageAsync(string botName, string message)
    {
        var bot = _config.Bots.FirstOrDefault(b => b.Name == botName);
        if (bot == null)
        {
            _logger.LogError($"Bot {botName} not found.");
            return;
        }

        var client = new TelegramBotClient(bot.Token);
        var success = false;

        for (int i = 0; i < _config.Settings.RetryCount; i++)
        {
            try
            {
                await client.SendTextMessageAsync(bot.DefaultChatId, message);
                success = true;
                break;
            }
            catch (ApiRequestException ex)
            {
                _logger.LogWarning($"Attempt {i + 1} failed: {ex.Message}");
                await Task.Delay(_config.Settings.RetryDelaySeconds * 1000);
            }
        }

        _db.MessageLogs.Add(new MessageLog
        {
            BotName = bot.Name,
            ChatId = bot.DefaultChatId,
            Message = message,
            Timestamp = DateTime.UtcNow,
            Success = success
        });

        await _db.SaveChangesAsync();
    }
}
