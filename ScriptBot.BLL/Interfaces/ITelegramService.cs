namespace ScriptBot.BLL.Interfaces
{
    public interface ITelegramService
    {
        Task SendMessageAsync(long chatId, string message, bool markdown = false);
    }
}