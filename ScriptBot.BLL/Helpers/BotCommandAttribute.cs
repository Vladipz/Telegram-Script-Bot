using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Helpers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BotCommandAttribute(string command, string description, params UserRole[] allowedRoles) : Attribute
    {
        public string Command { get; } = command;

        public string Description { get; } = description;

        public UserRole[] AllowedRoles { get; } = allowedRoles;

        public bool HasAccess(UserRole role)
        {
            return AllowedRoles.Contains(role);
        }
    }
}