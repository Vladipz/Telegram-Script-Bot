using System.Text;

using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    [BotCommand("/users", "List all users", UserRole.Admin)]
    public class ListUsersCommand : IBotCommand
    {
        private readonly IUserService _userService;

        public ListUsersCommand(IUserService userService)
        {
            _userService = userService;
        }

        public string Command => "/users";

        public async Task<ErrorOr<IEnumerable<TargetMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var users = await _userService.GetUsersAsync();

            if (!users.Any())
            {
                return Error.NotFound("UsersNotFound", "No users found.");
            }

            return GetSuccessMessages(telegramUpdate, users);
        }

        private static List<TargetMessageModel> GetSuccessMessages(TelegramUpdateModel telegramUpdate, IEnumerable<User> users)
        {
            var messageBuilder = new StringBuilder("Users list:\n\n");

            var i = 1;
            foreach (var user in users)
            {
                messageBuilder.AppendLine($"{i++}. @{user.Username} (ChatId: {user.ChatId}) - Role: {user.Role}");
            }

            // Повертаємо список із одного TargetMessageModel
            return
            [
                new TargetMessageModel(telegramUpdate.ChatId, messageBuilder.ToString())
            ];
        }
    }
}