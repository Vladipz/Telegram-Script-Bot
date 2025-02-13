using ScriptBot.BLL.Commands;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

namespace ScriptBot.BLL.Services
{
    public class CommandService : ICommandService
    {
        private readonly Dictionary<string, IBotCommand> _commands;

        public CommandService(IEnumerable<IBotCommand> commands)
        {
            _commands = commands.ToDictionary(cmd => cmd.Command, cmd => cmd);
        }

        public async Task HandleCommandAsync(TelegramUpdate telegramUpdate)
        {
            var parts = telegramUpdate.MessageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var command = parts[0];

            var args = parts.Skip(1).ToArray();

            if (_commands.TryGetValue(command, out var cmd))
            {
                await cmd.ExecuteAsync(telegramUpdate, args);
            }
        }
    }
}