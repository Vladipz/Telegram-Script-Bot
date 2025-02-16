using System.Text;

using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    [BotCommand("/lastuploads", "Show last uploads", UserRole.Admin)]
    public class LastUploadsCommand : IBotCommand
    {
        private readonly IUploadsService _uploadsService;

        public LastUploadsCommand(IUploadsService uploadsService)
        {
            _uploadsService = uploadsService;
        }

        public string Command => "/lastuploads";

        public async Task<ErrorOr<IEnumerable<TelegramMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var validationResult = ValidateAndExtractArguments(args);

            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }

            var updates = await _uploadsService.GetLastUploadsAsync(validationResult.Value);

            return GetSuccessMessages(telegramUpdate.ChatId, updates);
        }

        private static ErrorOr<IEnumerable<TelegramMessageModel>> GetSuccessMessages(long chatId, IEnumerable<Upload> updates)
        {
            var messageBuilder = new StringBuilder();

            if (!updates.Any())
            {
                messageBuilder.Append("No uploads found.");
            }
            else
            {
                foreach (var (upload, index) in updates.OrderByDescending(u => u.CreatedAt).Select((u, i) => (u, i + 1)))
                {
                    messageBuilder.AppendLine($"{index}. {upload.AppName} (User: @{upload.User.Username})");
                    messageBuilder.AppendLine($"🕒 {upload.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
                    messageBuilder.AppendLine();
                }
            }

            return new List<TelegramMessageModel> { new(chatId, messageBuilder.ToString().Trim()) };
        }

        private static ErrorOr<int> ValidateAndExtractArguments(string[] args)
        {
            var errors = new List<Error>();
            if (args.Length > 1)
            {
                errors.Add(Error.Validation("InvalidArguments", "Usage: /lastuploads <number>"));
            }

            // if cpunt is not provided, return 10
            if (args.Length == 0)
            {
                return 10;
            }

            if (!int.TryParse(args.ElementAtOrDefault(0), out int lastUploadsCount))
            {
                errors.Add(Error.Validation("InvalidNumber", "Number is invalid or missing."));
            }

            if (lastUploadsCount <= 0)
            {
                errors.Add(Error.Validation("InvalidNumber", "Number should be greater than 0."));
            }

            if (errors.Count != 0)
            {
                return errors;
            }

            return lastUploadsCount;
        }
    }
}