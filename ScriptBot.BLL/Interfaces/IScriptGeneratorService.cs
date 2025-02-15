using ErrorOr;

using ScriptBot.BLL.Models.Script;

namespace ScriptBot.BLL.Interfaces
{
    public interface IScriptGeneratorService
    {
        Task<ErrorOr<ScriptGenerationResult>> GenerateScriptAsync(ScriptGenerationRequest request);

        Task<ErrorOr<ScriptGenerationResult>> GenerateAndUploadScriptAsync(ScriptGenerationRequest request, SftpConnectionInfo connectionInfo);
    }
}