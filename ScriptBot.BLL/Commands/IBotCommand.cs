using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Commands
{
    public interface IBotCommand
    {
        string Command { get; }

        Task ExecuteAsync(TelegramUpdate telegramUpdate, string[] args);
    }
}