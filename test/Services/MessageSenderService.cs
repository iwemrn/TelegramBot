using Microsoft.Extensions.Options;
using Telegram.Bot;
using test.Models;

public class MessageSenderService
{
    private readonly AppDbContext _db;
    private readonly ILogger<MessageSenderService> _logger;
    private readonly List<BotConfig> _bots;
    private readonly ServiceSettings _settings;

    public MessageSenderService(IOptions<AppSettings> config, AppDbContext db, ILogger<MessageSenderService> logger)
    {
        _db = db;
        _logger = logger;
        _bots = config.Value.Bots;
        _settings = config.Value.Settings;
    }

    public async Task SendMessageAsync(string botName, string message)
    {
        var botConfig = _bots.FirstOrDefault(b => b.Name == botName);
        if (botConfig == null)
        {
            _logger.LogWarning("Бот {BotName} не найден", botName);
            return;
        }

        var botClient = new TelegramBotClient(botConfig.Token);
        bool success = false;
        string? error = null;

        for (int attempt = 1; attempt <= _settings.RetryCount; attempt++)
        {
            try
            {
                await botClient.SendTextMessageAsync(botConfig.DefaultChatId, message);
                success = true;
                break;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                _logger.LogError("Попытка {Attempt} не удалась: {Error}", attempt, ex.Message);
                await Task.Delay(_settings.RetryDelaySeconds * 1000);
            }
        }

        _db.messagelogs.Add(new MessageLog
        {
            BotName = botName,
            ChatId = botConfig.DefaultChatId,
            Message = message,
            SentAt = DateTime.UtcNow,
            IsSuccess = success
        });

        await _db.SaveChangesAsync();
    }

    public async Task RunInteractiveAsync()
    {
        Console.WriteLine("Доступные боты:");
        for (int i = 0; i < _bots.Count; i++)
        {
            var bot = _bots[i];
            Console.WriteLine($"{i + 1}. {bot.Name} → ChatId: {bot.DefaultChatId}");
        }

        Console.Write("\nВведите номер бота: ");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > _bots.Count)
        {
            Console.WriteLine("❌ Неверный выбор. Завершение.");
            return;
        }

        var selectedBot = _bots[choice - 1];
        Console.WriteLine($"✅ Бот {selectedBot.Name} выбран.\n");

        while (true)
        {
            Console.Write("Введите сообщение (или 'exit' для выхода): ");
            var message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message) || message.Trim().ToLower() == "exit")
                break;

            await SendMessageAsync(selectedBot.Name, message);
        }

        Console.WriteLine("🔚 Завершено.");
    }
}
