using Microsoft.Extensions.Logging;

using ScriptBot.BLL.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ScriptBot.BLL.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(ITelegramBotClient botClient, ILogger<TelegramService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task SendMessageAsync(long chatId, string message, bool markdown = false)
        {
            _ = await _botClient.SendMessage(
                chatId,
                message,
                parseMode: markdown ? ParseMode.Markdown : ParseMode.None);

            _logger.LogInformation("Sent message to chat {ChatId}", chatId);
        }
    }
}