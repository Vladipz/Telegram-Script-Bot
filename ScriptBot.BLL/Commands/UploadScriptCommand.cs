using ErrorOr;

using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Script;
using ScriptBot.BLL.Models.Telegram;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Commands
{
    [BotCommand("/upload_script", "Upload script to server", UserRole.User, UserRole.Admin)]
    public class UploadScriptCommand : IBotCommand
    {
        private readonly IScriptGeneratorService _scriptGeneratorService;
        private readonly IUserService _userService;
        private readonly IUploadsService _uploadsService;

        public UploadScriptCommand(
            IScriptGeneratorService scriptGenerator,
            IUserService userService,
            IUploadsService uploadsService)
        {
            _scriptGeneratorService = scriptGenerator;
            _userService = userService;
            _uploadsService = uploadsService;
        }

        public string Command => "/upload_script";

        public async Task<ErrorOr<IEnumerable<TelegramMessageModel>>> ExecuteAsync(TelegramUpdateModel telegramUpdate, string[] args)
        {
            var validationResult = ValidateAndExtractArguments(args);
            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }

            var (appName, appBundle, host, username, password) = validationResult.Value;

            var scriptResut = await GenerateAndUploadScript(appName, appBundle, host, username, password);

            if (scriptResut.IsError)
            {
                return scriptResut.Errors;
            }

            var userResult = await _userService.GetUserAsync(telegramUpdate.ChatId);

            if (userResult.IsError)
            {
                return userResult.Errors;
            }

            var scriptInfo = scriptResut.Value;

            var uploadResult = await SaveUploadInfo(userResult.Value.Id, scriptInfo, host, username);
            if (uploadResult.IsError)
            {
                return uploadResult.Errors;
            }

            return scriptResut.Match<ErrorOr<IEnumerable<TelegramMessageModel>>>(
                success => GetSuccessMessages(telegramUpdate, success),
                errors => errors);
        }

        private static List<TelegramMessageModel> GetSuccessMessages(TelegramUpdateModel telegramUpdate, ScriptGenerationResult scriptInfo)
        {
            return
            [
                new TelegramMessageModel(
                    telegramUpdate.ChatId,
                    $"Script uploaded successfully!\n" + $"Secret: {scriptInfo.Secret}\n" + $"Secret Key Param: {scriptInfo.SecretKeyParam}\n" + $"File: {scriptInfo.FileName}"),
            ];
        }

        private static ScriptGenerationRequest CreateScriptGenerationRequest(string appName, string appBundle)
        {
            return new ScriptGenerationRequest
            {
                AppName = appName,
                AppBundle = appBundle,
            };
        }

        private static SftpConnectionInfo CreateSftpConnectionInfo(string host, string username, string password)
        {
            return new SftpConnectionInfo
            {
                Host = host,
                Port = 22,
                Username = username,
                Password = password,
                RemotePath = "/upload",
            };
        }

        private static ErrorOr<(string AppName, string AppBundle, string Host, string Username, string Password)> ValidateAndExtractArguments(string[] args)
        {
            var errors = new List<Error>();

            if (args.Length != 5)
            {
                errors.Add(Error.Validation(
                    "InvalidArguments",
                    "Usage: /upload_script <appName> <appBundle> <host> <username> <password>"));
                return errors;
            }

            var appName = args[0];
            var appBundle = args[1];
            var host = args[2];
            var username = args[3];
            var password = args[4];

            // Валідація AppName та AppBundle
            if (string.IsNullOrWhiteSpace(appName))
            {
                errors.Add(Error.Validation(
                    "InvalidAppName",
                    "AppName is required."));
            }

            if (string.IsNullOrWhiteSpace(appBundle))
            {
                errors.Add(Error.Validation(
                    "InvalidAppBundle",
                    "AppBundle is required."));
            }
            else if (!appBundle.Contains('.'))
            {
                errors.Add(Error.Validation(
                    "InvalidAppBundleFormat",
                    "AppBundle should be in format 'com.example.app'"));
            }

            // Валідація параметрів сервера
            if (string.IsNullOrWhiteSpace(host))
            {
                errors.Add(Error.Validation(
                    "InvalidHost",
                    "Host is required."));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add(Error.Validation(
                    "InvalidUsername",
                    "Username is required."));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add(Error.Validation(
                    "InvalidPassword",
                    "Password is required."));
            }

            if (errors.Count != 0)
            {
                return errors;
            }

            return (appName, appBundle, host, username, password);
        }

        private async Task<ErrorOr<Success>> SaveUploadInfo(Guid userId, ScriptGenerationResult scriptInfo, string host, string username)
        {
            var upload = new Upload
            {
                UserId = userId,
                AppName = scriptInfo.UploadInfo.AppName,
                AppBundle = scriptInfo.UploadInfo.AppBundle,
                Secret = scriptInfo.Secret,
                SecretKeyParam = scriptInfo.SecretKeyParam,
                ServerHost = host,
                ServerUsername = username,
                ServerFilePath = Path.Combine(host, scriptInfo.FileName),
            };

            return await _uploadsService.AddUploadAsync(upload);
        }

        private async Task<ErrorOr<ScriptGenerationResult>> GenerateAndUploadScript(string appName, string appBundle, string host, string username, string password)
        {
            var request = CreateScriptGenerationRequest(appName, appBundle);
            var connectionInfo = CreateSftpConnectionInfo(host, username, password);
            return await _scriptGeneratorService.GenerateAndUploadScriptAsync(request, connectionInfo);
        }
    }
}