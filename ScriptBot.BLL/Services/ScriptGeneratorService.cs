using ErrorOr;

using Microsoft.Extensions.Logging;

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
        private readonly ILogger<ScriptGeneratorService> _logger;

        public ScriptGeneratorService(ISftpService sftpService, ILogger<ScriptGeneratorService> logger)
        {
            _sftpService = sftpService;
            _logger = logger;
        }

        public async Task<ErrorOr<ScriptGenerationResult>> GenerateScriptAsync(ScriptGenerationRequest request)
        {
            try
            {
                _logger.LogInformation("Starting script generation for app: {AppName}", request.AppName);

                ErrorOr<Success> validationResult = ValidateRequest(request);
                if (validationResult.IsError)
                {
                    _logger.LogWarning("Validation failed for app: {AppName}. Errors: {Errors}", request.AppName, string.Join(", ", validationResult.Errors));
                    return validationResult.Errors;
                }

                string sanitizedAppName = EscapePhpString(request.AppName);
                string sanitizedAppBundle = EscapePhpString(request.AppBundle);
                string secret = GenerateSecret();
                string secretKeyParam = GenerateSecretKeyParam();

                string scriptContent = ScriptTemplate
                    .Replace("[APP_NAME]", sanitizedAppName)
                    .Replace("[APP_BUNDLE]", sanitizedAppBundle)
                    .Replace("[SECRET]", secret)
                    .Replace("[SECRET_KEY_PARAM]", secretKeyParam);

                string fileName = GenerateFileName(request.AppName);

                Upload uploadInfo = new()
                {
                    AppName = request.AppName,
                    AppBundle = request.AppBundle,
                    Secret = secret,
                    SecretKeyParam = secretKeyParam,
                    ServerFilePath = fileName,
                };

                _logger.LogInformation("Script generated successfully for app: {AppName}, fileName: {FileName}", request.AppName, fileName);

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
                _logger.LogError(ex, "Script generation failed for app: {AppName}", request.AppName);
                return Error.Failure("ScriptGeneration.Failed", ex.Message);
            }
        }

        public async Task<ErrorOr<ScriptGenerationResult>> GenerateAndUploadScriptAsync(
            ScriptGenerationRequest request,
            SftpConnectionInfo connectionInfo)
        {
            _logger.LogInformation("Starting script generation and upload for app: {AppName}", request.AppName);

            ErrorOr<ScriptGenerationResult> generationResult = await GenerateScriptAsync(request);
            if (generationResult.IsError)
            {
                _logger.LogWarning("Script generation failed for app: {AppName}", request.AppName);
                return generationResult.Errors;
            }

            _logger.LogInformation("Uploading script for app: {AppName}", request.AppName);

            ErrorOr<Success> uploadResult = await _sftpService.UploadFileAsync(
                generationResult.Value.ScriptContent,
                generationResult.Value.FileName,
                connectionInfo);

            if (uploadResult.IsError)
            {
                _logger.LogError("Script upload failed for app: {AppName}, fileName: {FileName}", request.AppName, generationResult.Value.FileName);
                return uploadResult.Errors;
            }

            _logger.LogInformation("Script successfully generated and uploaded for app: {AppName}", request.AppName);
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