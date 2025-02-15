using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Models.Script
{
    public record ScriptGenerationResult
    {
        public string ScriptContent { get; init; }

        public string Secret { get; init; }

        public string SecretKeyParam { get; init; }

        public string FileName { get; init; }

        public Upload UploadInfo { get; init; }
    }
}