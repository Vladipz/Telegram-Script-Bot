using ErrorOr;

using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Defines the contract for command handling services.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Handles a command received from a Telegram update.
        /// </summary>
        /// <param name="telegramUpdate">The Telegram update containing the command.</param>
        /// <returns>A task that represents the asynchronous operation, containing the result of the command handling.</returns>
        Task<ErrorOr<IEnumerable<TelegramMessageModel>>> HandleCommandAsync(TelegramUpdateModel telegramUpdate);
    }
}