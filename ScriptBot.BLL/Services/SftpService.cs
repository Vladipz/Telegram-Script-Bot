using ErrorOr;

using Microsoft.Extensions.Logging;

using Renci.SshNet;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Services
{
    public class SftpService : ISftpService
    {
        private readonly ILogger<SftpService> _logger;

        public SftpService(ILogger<SftpService> logger)
        {
            _logger = logger;
        }

        public async Task<ErrorOr<Success>> UploadFileAsync(string content, string fileName, SftpConnectionInfo connectionInfo)
        {
            _logger.LogInformation("Starting SFTP upload for file: {FileName}", fileName);

            ErrorOr<SftpClient> clientResult = ConnectToServer(connectionInfo);
            if (clientResult.IsError)
            {
                _logger.LogError("Failed to connect to SFTP server: {Host}:{Port}", connectionInfo.Host, connectionInfo.Port);
                return clientResult.FirstError;
            }

            using SftpClient client = clientResult.Value;

            ErrorOr<string> tempFile = await CreateTempFileAsync(content);
            if (tempFile.IsError)
            {
                _logger.LogError("Failed to create temporary file for upload");
                return tempFile.FirstError;
            }

            try
            {
                return await UploadFileToServerAsync(client, tempFile.Value, fileName);
            }
            finally
            {
                await CleanupAsync(tempFile.Value, client);
            }
        }

        private ErrorOr<SftpClient> ConnectToServer(SftpConnectionInfo connectionInfo)
        {
            try
            {
                _logger.LogInformation("Connecting to SFTP server: {Host}:{Port}", connectionInfo.Host, connectionInfo.Port);

                SftpClient client = new SftpClient(
                    connectionInfo.Host,
                    connectionInfo.Port,
                    connectionInfo.Username,
                    connectionInfo.Password);

                client.Connect();

                if (!client.IsConnected)
                {
                    _logger.LogError("Failed to establish SFTP connection to {Host}:{Port}", connectionInfo.Host, connectionInfo.Port);
                    return Error.Failure("Sftp.ConnectionFailed", "Failed to connect to SFTP server");
                }

                _logger.LogInformation("Successfully connected to SFTP server");
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SFTP connection error: {ErrorMessage}", ex.Message);
                return Error.Failure("Sftp.ConnectionFailed", ex.Message);
            }
        }

        private async Task<ErrorOr<string>> CreateTempFileAsync(string content)
        {
            try
            {
                string tempPath = Path.GetRandomFileName();
                _logger.LogInformation("Creating temporary file: {TempPath}", tempPath);

                await File.WriteAllTextAsync(tempPath, content);
                return tempPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create temporary file: {ErrorMessage}", ex.Message);
                return Error.Failure("Sftp.TempFileCreationFailed", ex.Message);
            }
        }

        private async Task<ErrorOr<Success>> UploadFileToServerAsync(
            SftpClient client,
            string tempPath,
            string fileName)
        {
            try
            {
                string remoteFilePath = $"/upload/{fileName}";
                _logger.LogInformation("Starting file upload to: {RemoteFilePath}", remoteFilePath);

                if (!await client.ExistsAsync("/upload"))
                {
                    _logger.LogError("Upload directory not found on SFTP server");
                    return Error.Failure("Sftp.DirectoryNotFound", "Upload directory not found");
                }

                using FileStream fileStream = File.OpenRead(tempPath);
                client.UploadFile(fileStream, remoteFilePath);

                _logger.LogInformation("Successfully uploaded file to: {RemoteFilePath}", remoteFilePath);
                return Result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload failed: {ErrorMessage}", ex.Message);
                return Error.Failure("Sftp.UploadFailed", ex.Message);
            }
        }

        private Task CleanupAsync(string tempPath, SftpClient client)
        {
            if (File.Exists(tempPath))
            {
                _logger.LogInformation("Cleaning up temporary file: {TempPath}", tempPath);
                File.Delete(tempPath);
            }

            if (client.IsConnected)
            {
                _logger.LogInformation("Disconnecting from SFTP server");
                client.Disconnect();
            }

            return Task.CompletedTask;
        }
    }
}