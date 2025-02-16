using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Mappings;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    [BotCommand("/start", "Register user", UserRole.Guest, UserRole.User, UserRole.Admin)]
    public class StartCommand : IBotCommand
    {
        private readonly IUserService _userService;

        public StartCommand(IUserService userService)
        {
            _userService = userService;
        }

        public string Command => "/start";

        public async Task<ErrorOr<IEnumerable<TelegramMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var result = await _userService.CreateUserAsync(telegramUpdate.ToCreateModel(UserRole.User));

            return result.Match<ErrorOr<IEnumerable<TelegramMessageModel>>>(
                created => GetSuccessMessages(telegramUpdate),
                errors => errors);
        }

        private static List<TelegramMessageModel> GetSuccessMessages(TelegramUpdateModel telegramUpdate)
        {
            return
            [
                new TelegramMessageModel(telegramUpdate.ChatId, "You have been registered!"),
            ];
        }
    }
}