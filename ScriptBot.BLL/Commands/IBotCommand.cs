using ErrorOr;

using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Commands
{
    public interface IBotCommand
    {
        string Command { get; }

        Task<ErrorOr<IEnumerable<TargetMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args);
    }
}