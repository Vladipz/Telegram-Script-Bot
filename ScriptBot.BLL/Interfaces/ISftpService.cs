using ErrorOr;

using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for SFTP file transfer operations.
    /// </summary>
    public interface ISftpService
    {
        /// <summary>
        /// Uploads a file to an SFTP server asynchronously.
        /// </summary>
        /// <param name="content">The content of the file to upload.</param>
        /// <param name="fileName">The name of the file to be created on the SFTP server.</param>
        /// <param name="connectionInfo">Connection details for the SFTP server.</param>
        /// <returns>A success result if the upload was successful, or an error if it failed.</returns>
        Task<ErrorOr<Success>> UploadFileAsync(
            string content,
            string fileName,
            SftpConnectionInfo connectionInfo);
    }
}