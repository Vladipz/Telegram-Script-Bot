using ScriptBot.DAL.Interfaces;

namespace ScriptBot.DAL.Entities
{
    public class Upload : IDatedEntity
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string AppName { get; set; } = string.Empty;

        public string AppBundle { get; set; } = string.Empty;

        public string Secret { get; set; } = string.Empty;

        public string SecretKeyParam { get; set; } = string.Empty;

        public string ServerHost { get; set; } = string.Empty;

        public string ServerUsername { get; set; } = string.Empty;

        public string ServerFilePath { get; set; } = string.Empty;

        public User User { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}