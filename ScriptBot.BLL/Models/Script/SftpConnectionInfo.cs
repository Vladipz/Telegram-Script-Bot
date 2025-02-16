namespace ScriptBot.BLL.Models.Script
{
    public record SftpConnectionInfo
    {
        public string Host { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public int Port { get; init; } = 22;

        public string RemotePath { get; init; } = "/upload";
    }
}