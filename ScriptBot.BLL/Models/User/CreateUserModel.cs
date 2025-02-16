using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Models.User
{
    public class CreateUserModel
    {
        public long ChatId { get; set; } // Telegram ChatId

        public string Username { get; set; } = string.Empty;

        public UserRole Role { get; set; } // enum
    }
}