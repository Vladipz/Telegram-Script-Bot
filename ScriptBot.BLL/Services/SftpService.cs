using ErrorOr;

using Renci.SshNet;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Services
{
    public class SftpService : ISftpService
    {
        public SftpService()
        {
        }

        public async Task<ErrorOr<Success>> UploadFileAsync(string content, string fileName, SftpConnectionInfo connectionInfo)
        {
            var clientResult = ConnectToServer(connectionInfo);
            if (clientResult.IsError)
            {
                return clientResult.FirstError;
            }

            using var client = clientResult.Value;

            var tempFile = await CreateTempFileAsync(content);
            if (tempFile.IsError)
            {
                return tempFile.FirstError;
            }

            try
            {
                return await UploadFileToServerAsync(clientResult.Value, tempFile.Value, fileName);
            }
            finally
            {
                await CleanupAsync(tempFile.Value, clientResult.Value);
            }
        }

        private static ErrorOr<SftpClient> ConnectToServer(SftpConnectionInfo connectionInfo)
        {
            try
            {
                var client = new SftpClient(
                    connectionInfo.Host,
                    connectionInfo.Port,
                    connectionInfo.Username,
                    connectionInfo.Password);

                client.Connect();

                return !client.IsConnected
                    ? Error.Failure("Sftp.ConnectionFailed", "Failed to connect to SFTP server")
                    : client;
            }
            catch (Exception ex)
            {
                return Error.Failure("Sftp.ConnectionFailed", ex.Message);
            }
        }

        private static async Task<ErrorOr<string>> CreateTempFileAsync(string content)
        {
            try
            {
                var tempPath = Path.GetRandomFileName();
                await File.WriteAllTextAsync(tempPath, content);
                return tempPath;
            }
            catch (Exception ex)
            {
                return Error.Failure("Sftp.TempFileCreationFailed", ex.Message);
            }
        }

        private static async Task<ErrorOr<Success>> UploadFileToServerAsync(
            SftpClient client,
            string tempPath,
            string fileName)
        {
            try
            {
                var remoteFilePath = $"/upload/{fileName}";

                if (!await client.ExistsAsync("/upload"))
                {
                    return Error.Failure("Sftp.DirectoryNotFound", "Upload directory not found");
                }

                using var fileStream = File.OpenRead(tempPath);
                client.UploadFile(fileStream, remoteFilePath);

                return Result.Success;
            }
            catch (Exception ex)
            {
                return Error.Failure("Sftp.UploadFailed", ex.Message);
            }
        }

        private static Task CleanupAsync(string tempPath, SftpClient client)
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            if (client.IsConnected)
            {
                client.Disconnect();
            }

            return Task.CompletedTask;
        }
    }
}