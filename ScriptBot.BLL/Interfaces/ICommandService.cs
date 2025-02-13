using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Interfaces
{
    public interface ICommandService
    {
        Task HandleCommandAsync(TelegramUpdate telegramUpdate);
    }
}