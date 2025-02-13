using ErrorOr;

using Microsoft.EntityFrameworkCore;

using ScriptBot.BLL.Interfaces;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class RoleManagerService : IRoleManagerService
    {
        private readonly BotDbContext _dbContext;

        public RoleManagerService(BotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<UserRole>> GetUserRoleAsync(long chatId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

            if (user == null)
            {
                return Error.NotFound("User not found");
            }

            return user.Role;
        }

        public async Task<ErrorOr<IEnumerable<User>>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _dbContext.Users.Where(u => u.Role == role).ToListAsync();

            return users;
        }

        public async Task<ErrorOr<Updated>> UpdateUserRoleAsync(Guid userId, UserRole role)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Error.NotFound("User not found");
            }

            user.Role = role;

            _ = await _dbContext.SaveChangesAsync();

            return Result.Updated;
        }

        public async Task<ErrorOr<Updated>> UpdateUserRoleAsync(long chatId, UserRole role)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user == null)
            {
                return Error.NotFound(description: "User not found");
            }

            if (user.Role == role)
            {
                return Error.Conflict(description: "User already has this role");
            }

            user.Role = role;

            _ = await _dbContext.SaveChangesAsync();

            return Result.Updated;
        }
    }
}