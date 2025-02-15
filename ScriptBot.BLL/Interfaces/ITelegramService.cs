namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for sending messages through Telegram.
    /// </summary>
    public interface ITelegramService
    {
        /// <summary>
        /// Sends a message to a Telegram chat asynchronously.
        /// </summary>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <param name="message">The text message to send.</param>
        /// <param name="markdown">Whether to parse the message as Markdown. Defaults to false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendMessageAsync(long chatId, string message, bool markdown = false);
    }
}