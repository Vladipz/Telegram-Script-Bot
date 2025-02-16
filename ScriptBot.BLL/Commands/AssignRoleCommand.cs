using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    [BotCommand("/assignrole", "Assign role to user", UserRole.Admin)]
    public class AssignRoleCommand : IBotCommand
    {
        private readonly IRoleManagerService _roleManager;

        public AssignRoleCommand(
            IRoleManagerService roleManager)
        {
            _roleManager = roleManager;
        }

        public string Command => "/assignrole";

        public async Task<ErrorOr<IEnumerable<TelegramMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var validationResult = ValidateArguments(args);
            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }

            var (targetChatId, newRole) = validationResult.Value;

            var result = await _roleManager.UpdateUserRoleAsync(targetChatId, newRole);

            return result.Match<ErrorOr<IEnumerable<TelegramMessageModel>>>(
                updated => GetSuccessMessages(telegramUpdate.ChatId, targetChatId, newRole),
                errors => errors);
        }

        private static List<TelegramMessageModel> GetSuccessMessages(long chatId, long targetChatId, UserRole newRole)
        {
            return
            [
                new TelegramMessageModel(chatId, $"Role {newRole} successfully assigned to user with ChatId: {targetChatId}"),
                new TelegramMessageModel(targetChatId, $"Your role has been changed to {newRole}"),
            ];
        }

        private static ErrorOr<(long TargetChatId, UserRole NewRole)> ValidateArguments(string[] args)
        {
            var errors = new List<Error>(); // Список для накопичення помилок

            if (args.Length != 2)
            {
                errors.Add(Error.Validation("InvalidArguments", "Usage: /assignrole <chatId> <role>"));
            }

            if (!long.TryParse(args.ElementAtOrDefault(0), out long targetChatId))
            {
                errors.Add(Error.Validation("InvalidChatId", "ChatId is invalid or missing."));
            }

            if (!Enum.TryParse(args.ElementAtOrDefault(1), true, out UserRole newRole))
            {
                errors.Add(Error.Validation("InvalidRole", "Role is invalid or missing."));
            }

            if (errors.Count != 0)
            {
                return errors; // Якщо є хоча б одна помилка, повертаємо сукупність помилок
            }

            return (targetChatId, newRole); // Успішний результат
        }
    }
}