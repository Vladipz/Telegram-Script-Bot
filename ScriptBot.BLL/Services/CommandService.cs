using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Services
{
    public class CommandService : ICommandService
    {
        private readonly CommandRegistry _commandRegistry;
        private readonly IUserService _userService;

        public CommandService(CommandRegistry commandRegistry, IUserService userService)
        {
            _commandRegistry = commandRegistry;
            _userService = userService;
        }

        public async Task<ErrorOr<IEnumerable<TelegramMessageModel>>> HandleCommandAsync(TelegramUpdateModel telegramUpdate)
        {
            var commandText = telegramUpdate.MessageText.Split(' ')[0];

            var args = telegramUpdate.MessageText.Split(' ').Skip(1).ToArray();

            var role = await _userService.GetUserRoleAsync(telegramUpdate.ChatId);

            var command = _commandRegistry.GetCommand(commandText, role);

            if (command == null)
            {
                return Error.NotFound("CommandNotFound", "Unknown command.");
            }

            return await command.ExecuteAsync(telegramUpdate, args);
        }
    }
}