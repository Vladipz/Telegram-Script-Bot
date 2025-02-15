using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ScriptBot.BLL.Commands;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Helpers
{
    public class CommandRegistry
    {
        private readonly List<IBotCommand> _commands;
        private readonly ILogger<CommandRegistry> _logger;

        public CommandRegistry(IServiceProvider serviceProvider, ILogger<CommandRegistry> logger)
        {
            _logger = logger;

            _commands = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBotCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IBotCommand)ActivatorUtilities.CreateInstance(serviceProvider, t))
                .ToList();

            _logger.LogInformation("Loaded {CommandCount} bot commands.", _commands.Count); // Використовуємо LogInformation
        }

        public IEnumerable<IBotCommand> GetAvailableCommands(UserRole role)
        {
            var availableCommands = _commands
                .Where(cmd => cmd.GetType().GetCustomAttribute<BotCommandAttribute>()?.HasAccess(role) ?? false)
                .ToList();

            _logger.LogInformation("User with role {UserRole} has access to {CommandCount} commands.", role, availableCommands.Count); // Використовуємо LogInformation

            return availableCommands;
        }

        public IBotCommand? GetCommand(string commandText, UserRole role)
        {
            _logger.LogInformation("Searching for command: {CommandText} with role: {UserRole}", commandText, role); // Використовуємо LogInformation

            foreach (var cmd in _commands)
            {
                var commandType = cmd.GetType();
                var commandAttribute = commandType.GetCustomAttribute<BotCommandAttribute>();

                _logger.LogDebug("Checking command: {Command}, Attribute: {AttributeCommand}, AllowedRoles: {AllowedRoles}",
                    cmd.Command,
                    commandAttribute?.Command,
                    string.Join(", ", commandAttribute?.AllowedRoles ?? Array.Empty<UserRole>())); // Використовуємо LogDebug

                if (cmd.Command == commandText && commandAttribute?.HasAccess(role) == true)
                {
                    _logger.LogInformation("Match found for command: {Command}", cmd.Command); // Використовуємо LogInformation
                    return cmd;
                }
            }

            _logger.LogWarning("No matching command found for: {CommandText} and role: {UserRole}", commandText, role); // Використовуємо LogWarning
            return null;
        }
    }
}