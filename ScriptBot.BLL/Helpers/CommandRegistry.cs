using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using ScriptBot.BLL.Commands;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Helpers
{
    public class CommandRegistry
    {
        private readonly List<IBotCommand> _commands;

        public CommandRegistry(IServiceProvider serviceProvider)
        {
            _commands = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBotCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => (IBotCommand)ActivatorUtilities.CreateInstance(serviceProvider, t))
                .ToList();
        }

        public IEnumerable<IBotCommand> GetAvailableCommands(UserRole role)
        {
            return _commands.Where(cmd =>
                cmd.GetType().GetCustomAttribute<BotCommandAttribute>()?.HasAccess(role) ?? false);
        }

        public IBotCommand? GetCommand(string commandText, UserRole role)
        {
            Console.WriteLine($"Searching for the command: {commandText} with role: {role}");

            foreach (var cmd in _commands)
            {
                var commandType = cmd.GetType();

                var commandAttribute = commandType.GetCustomAttribute<BotCommandAttribute>();

                Console.WriteLine($"Found command: {cmd.Command}");
                Console.WriteLine($"Attribute: {commandAttribute?.Command}, AllowedRoles: {string.Join(", ", commandAttribute?.AllowedRoles ?? Array.Empty<UserRole>())}");

                if (cmd.Command == commandText && commandAttribute?.HasAccess(role) == true)
                {
                    Console.WriteLine("Match found!");
                    return cmd;
                }
            }

            Console.WriteLine("No matching command found.");
            return null;
        }
    }
}