using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    public class AssignRoleCommand : IBotCommand
    {
        private readonly IRoleManagerService _roleManager;
        private readonly ITelegramService _telegramService;

        public AssignRoleCommand(
            IRoleManagerService roleManager,
            ITelegramService telegramService)
        {
            _roleManager = roleManager;
            _telegramService = telegramService;
        }

        public string Command => "/assignrole";

        public async Task ExecuteAsync(TelegramUpdate telegramUpdate, string[] args)
        {
            if (args.Length != 2)
            {
                await _telegramService.SendMessageAsync(
                    telegramUpdate.ChatId,
                    "Usage: /assignrole <chatId> <role>");
                return;
            }

            if (!long.TryParse(args[0], out long targetChatId))
            {
                await _telegramService.SendMessageAsync(
                    telegramUpdate.ChatId,
                    "Invalid ChatId format");
                return;
            }

            if (!Enum.TryParse(args[1], true, out UserRole newRole))
            {
                await _telegramService.SendMessageAsync(
                    telegramUpdate.ChatId,
                    "Invalid role. Available roles: admin, user");
                return;
            }

            var result = await _roleManager.UpdateUserRoleAsync(targetChatId, newRole);

            if (result.IsError)
            {
                // TODO: There are two possible error cases here:
                // FIX: Want top add ToRespose() method to the Result class
                await _telegramService.SendMessageAsync(
                    telegramUpdate.ChatId,
                    $"Failed to assign role. User with ChatId: {targetChatId} not found");
            }
            else
            {
                await NotifyAboutRoleChange(telegramUpdate.ChatId, targetChatId, newRole);
            }
        }

        private Task NotifyAboutRoleChange(long adminChatId, long targetChatId, UserRole newRole)
        {
            return Task.WhenAll(
                _telegramService.SendMessageAsync(
                    adminChatId,
                    $"Role {newRole} successfully assigned to user with ChatId: {targetChatId}"),
                _telegramService.SendMessageAsync(
                    targetChatId,
                    $"Your role has been changed to {newRole}"));
        }
    }
}