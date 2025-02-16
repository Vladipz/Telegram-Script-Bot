using ErrorOr;

using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for generating and managing scripts.
    /// </summary>
    public interface IScriptGeneratorService
    {
        /// <summary>
        /// Generates a script based on the provided request parameters.
        /// </summary>
        /// <param name="request">The script generation request containing necessary parameters.</param>
        /// <returns>A result containing either the generated script or an error.</returns>
        Task<ErrorOr<ScriptGenerationResult>> GenerateScriptAsync(ScriptGenerationRequest request);

        /// <summary>
        /// Generates a script and uploads it to an SFTP server.
        /// </summary>
        /// <param name="request">The script generation request containing necessary parameters.</param>
        /// <param name="connectionInfo">SFTP connection information for uploading the script.</param>
        /// <returns>A result containing either the generated and uploaded script or an error.</returns>
        Task<ErrorOr<ScriptGenerationResult>> GenerateAndUploadScriptAsync(ScriptGenerationRequest request, SftpConnectionInfo connectionInfo);
    }
}