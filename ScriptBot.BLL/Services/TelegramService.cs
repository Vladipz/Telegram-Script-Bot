using ScriptBot.BLL.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ScriptBot.BLL.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMessageAsync(long chatId, string message, bool markdown = false)
        {
            _ = await _botClient.SendMessage(
                chatId,
                message,
                parseMode: markdown ? ParseMode.Markdown : ParseMode.None);
        }
    }
}