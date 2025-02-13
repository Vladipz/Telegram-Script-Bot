using ErrorOr;

using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Interfaces
{
    public interface ICommandService
    {
        Task<ErrorOr<IEnumerable<TargetMessageModel>>> HandleCommandAsync(TelegramUpdateModel telegramUpdate);
    }
}