using ErrorOr;

using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Interfaces
{
    public interface ISftpService
    {
        Task<ErrorOr<Success>> UploadFileAsync(
            string content,
            string fileName,
            SftpConnectionInfo connectionInfo);
    }
}