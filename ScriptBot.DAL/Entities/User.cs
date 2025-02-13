namespace ScriptBot.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public long ChatId { get; set; } // Telegram ChatId

        public string Username { get; set; } = string.Empty;

        public UserRole Role { get; set; } // enum

        public bool IsBlocked { get; set; }

        public virtual ICollection<Upload> Uploads { get; set; } = [];
    }
}