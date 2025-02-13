using ErrorOr;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Mappings;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    public class StartCommand : IBotCommand
    {
        private readonly IUserService _userService;

        public StartCommand(IUserService userService)
        {
            _userService = userService;
        }

        public string Command => "/start";

        public async Task<ErrorOr<IEnumerable<TargetMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var result = await _userService.CreateUserAsync(telegramUpdate.ToCreateModel(UserRole.User));

            return result.Match<ErrorOr<IEnumerable<TargetMessageModel>>>(
                created => GetSuccessMessages(telegramUpdate),
                errors => errors);
        }

        private static List<TargetMessageModel> GetSuccessMessages(TelegramUpdateModel telegramUpdate)
        {
            return
            [
                new TargetMessageModel(telegramUpdate.ChatId, "You have been registered!"),
            ];
        }
    }
}