using ErrorOr;

using ScriptBot.BLL.Commands;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Services
{
    public class CommandService : ICommandService
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public CommandService(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        public async Task<ErrorOr<IEnumerable<TargetMessageModel>>> HandleCommandAsync(TelegramUpdateModel telegramUpdate)
        {
            var commandText = telegramUpdate.MessageText.Split(' ')[0];

            var args = telegramUpdate.MessageText.Split(' ').Skip(1).ToArray();

            var command = _commands.FirstOrDefault(c => c.Command == commandText);

            if (command == null)
            {
                return Error.NotFound("CommandNotFound", "Unknown command.");
            }

            return await command.ExecuteAsync(telegramUpdate, args);
        }
    }
}