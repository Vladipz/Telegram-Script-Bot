namespace ScriptBot.BLL.Models.Telegram
{
    public class TelegramUpdate
    {
        public long ChatId { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string MessageText { get; set; } = string.Empty;
    }
}