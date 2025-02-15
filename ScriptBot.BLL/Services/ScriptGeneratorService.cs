using ErrorOr;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Script;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class ScriptGeneratorService : IScriptGeneratorService
    {
        private const string ScriptTemplate = @"<?php
            $appName = '[APP_NAME]';
            $appBundle = '[APP_BUNDLE]';
            $secretKey = '[SECRET]';
            
            if($secretKey == $_GET['[SECRET_KEY_PARAM]']) {
                echo 'Привет я приложение '. $appName .' моя ссылка на гугл плей https://play.google.com/store/apps/details?id='. $appBundle;
            }
        ?>";

        private readonly ISftpService _sftpService;

        public ScriptGeneratorService(ISftpService sftpService)
        {
            _sftpService = sftpService;
        }

        public async Task<ErrorOr<ScriptGenerationResult>> GenerateScriptAsync(ScriptGenerationRequest request)
        {
            try
            {
                var validationResult = ValidateRequest(request);
                if (validationResult.IsError)
                {
                    return validationResult.Errors;
                }

                var sanitizedAppName = EscapePhpString(request.AppName);
                var sanitizedAppBundle = EscapePhpString(request.AppBundle);
                var secret = GenerateSecret();
                var secretKeyParam = GenerateSecretKeyParam();

                // Формування контенту скрипта
                string scriptContent = ScriptTemplate
                    .Replace("[APP_NAME]", sanitizedAppName)
                    .Replace("[APP_BUNDLE]", sanitizedAppBundle)
                    .Replace("[SECRET]", secret)
                    .Replace("[SECRET_KEY_PARAM]", secretKeyParam);

                var fileName = GenerateFileName(request.AppName);

                var uploadInfo = new Upload
                {
                    AppName = request.AppName,
                    AppBundle = request.AppBundle,
                    Secret = secret,
                    SecretKeyParam = secretKeyParam,
                    ServerFilePath = fileName,
                };

                return new ScriptGenerationResult
                {
                    ScriptContent = scriptContent,
                    Secret = secret,
                    SecretKeyParam = secretKeyParam,
                    FileName = fileName,
                    UploadInfo = uploadInfo,
                };
            }
            catch (Exception ex)
            {
                return Error.Failure("ScriptGeneration.Failed", ex.Message);
            }
        }

        public async Task<ErrorOr<ScriptGenerationResult>> GenerateAndUploadScriptAsync(
            ScriptGenerationRequest request,
            SftpConnectionInfo connectionInfo)
        {
            // Генеруємо скрипт
            var generationResult = await GenerateScriptAsync(request);
            if (generationResult.IsError)
            {
                return generationResult.Errors;
            }

            // Завантажуємо на сервер
            var uploadResult = await _sftpService.UploadFileAsync(
                generationResult.Value.ScriptContent,
                generationResult.Value.FileName,
                connectionInfo);

            if (uploadResult.IsError)
            {
                return uploadResult.Errors;
            }

            return generationResult;
        }

        private static string EscapePhpString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input
                .Replace("\\", "\\\\") // Backslash
                .Replace("'", "\\'") // Single quote
                .Replace("\"", "\\\"") // Double quote
                .Replace("\r", "\\r") // Carriage return
                .Replace("\n", "\\n") // New line
                .Replace("\t", "\\t") // Tab
                .Replace("$", "\\$");   // PHP variable
        }

        private static ErrorOr<Success> ValidateRequest(ScriptGenerationRequest request)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(request.AppName))
            {
                errors.Add(Error.Validation("AppName.Required", "AppName is required"));
            }

            if (string.IsNullOrWhiteSpace(request.AppBundle))
            {
                errors.Add(Error.Validation("AppBundle.Required", "AppBundle is required"));
            }

            if (!request.AppBundle.Contains("."))
            {
                errors.Add(Error.Validation(
                    "AppBundle.InvalidFormat",
                    "AppBundle should be in format 'com.example.app'"));
            }

            return errors.Any()
                ? errors.ToArray()
                : Result.Success;
        }

        private static string GenerateSecret()
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string GenerateSecretKeyParam()
        {
            var guid = Guid.NewGuid();
            var guidString = guid.ToString("N"); // N format: 32 digits
            return $"key_{guidString[..8]}"; // Take first 8 characters
        }

        private static string GenerateFileName(string appName)
        {
            // Формування імені файлу
            var sanitizedAppName = string.Join(
                "_",
                appName.Split(Path.GetInvalidFileNameChars()));

            return $"{sanitizedAppName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.php";
        }
    }
}