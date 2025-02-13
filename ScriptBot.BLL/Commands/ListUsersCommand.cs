using System.Text;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Commands
{
    public class ListUsersCommand : IBotCommand
    {
        private readonly IUserService _userService;
        private readonly ITelegramService _telegramService;

        public ListUsersCommand(IUserService userService, ITelegramService telegramService)
        {
            _userService = userService;
            _telegramService = telegramService;
        }

        public string Command => "/users";

        public async Task ExecuteAsync(TelegramUpdate update, string[] args)
        {
            var users = await _userService.GetUsersAsync();

            if (!users.Any())
            {
                await _telegramService.SendMessageAsync(update.ChatId, "No users found.");
                return;
            }

            var messageBuilder = new StringBuilder("Users list:\n\n");

            var i = 1;
            foreach (var user in users)
            {
                messageBuilder.AppendLine($"{i++}. @{user.Username} (ChatId: {user.ChatId}) - Role: {user.Role}");
            }

            await _telegramService.SendMessageAsync(update.ChatId, messageBuilder.ToString());
        }
    }
}