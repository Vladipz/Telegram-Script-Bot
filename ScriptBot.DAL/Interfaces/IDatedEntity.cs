namespace ScriptBot.DAL.Interfaces
{
    public interface IDatedEntity
    {
        DateTimeOffset CreatedAt { get; set; }

        DateTimeOffset UpdatedAt { get; set; }
    }
}