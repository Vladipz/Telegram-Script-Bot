using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    public class StartCommand : IBotCommand
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserService _userService;

        public StartCommand(ITelegramService telegramService, IUserService userService)
        {
            _telegramService = telegramService;
            _userService = userService;
        }

        public string Command => "/start";

        public async Task ExecuteAsync(TelegramUpdate telegramUpdate, string[] args)
        {
            var result = await _userService.CreateUserAsync(new CreateUserModel
            {
                ChatId = telegramUpdate.ChatId,
                Username = telegramUpdate.Username,
                Role = UserRole.Guest,
            });

            await result.Match(
                 user => _telegramService.SendMessageAsync(telegramUpdate.ChatId, $"You have been registered!"),
                 errors => _telegramService.SendMessageAsync(telegramUpdate.ChatId, errors[0].Description));
        }
    }
}