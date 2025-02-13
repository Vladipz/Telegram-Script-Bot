using ScriptBot.BLL.Models.Telegram;
using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Mappings
{
    public static class UserMappingExtentions
    {
        public static CreateUserModel ToCreateModel(this TelegramUpdateModel model, UserRole role = UserRole.User)
        {
            return new CreateUserModel
            {
                ChatId = model.ChatId,
                Username = model.Username,
                Role = role,
            };
        }
    }
}