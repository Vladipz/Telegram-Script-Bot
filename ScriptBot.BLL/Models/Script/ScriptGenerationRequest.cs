namespace ScriptBot.BLL.Models.Script
{
    public record ScriptGenerationRequest
    {
        public string AppName { get; init; } = string.Empty;

        public string AppBundle { get; init; } = string.Empty;
    }
}