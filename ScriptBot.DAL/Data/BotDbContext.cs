using Microsoft.EntityFrameworkCore;

using ScriptBot.DAL.Entities;

namespace ScriptBot.DAL.Data
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options)
            : base(options)
        {
        }

        public DbSet<Upload> Uploads { get; set; } = default!;

        public DbSet<User> Users { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<UserRole>(v));
        }
    }
}