using ErrorOr;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ScriptBot.BLL.Interfaces;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class RoleManagerService : IRoleManagerService
    {
        private readonly BotDbContext _dbContext;
        private readonly ILogger<RoleManagerService> _logger;

        public RoleManagerService(BotDbContext dbContext, ILogger<RoleManagerService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ErrorOr<UserRole>> GetUserRoleAsync(long chatId)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

            if (user is null)
            {
                _logger.LogWarning("User with ChatId {ChatId} not found", chatId);
                return Error.NotFound("User not found");
            }

            _logger.LogInformation("Retrieved role {Role} for user with ChatId {ChatId}", user.Role, chatId);
            return user.Role;
        }

        public async Task<ErrorOr<IEnumerable<User>>> GetUsersByRoleAsync(UserRole role)
        {
            List<User> users = await _dbContext.Users.Where(u => u.Role == role).ToListAsync();
            _logger.LogInformation("Retrieved {Count} users with role {Role}", users.Count, role);
            return users;
        }

        public async Task<ErrorOr<Updated>> UpdateUserRoleAsync(Guid userId, UserRole role)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                _logger.LogWarning("User with Id {UserId} not found", userId);
                return Error.NotFound("User not found");
            }

            user.Role = role;
            _ = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated role to {Role} for user with Id {UserId}", role, userId);
            return Result.Updated;
        }

        public async Task<ErrorOr<Updated>> UpdateUserRoleAsync(long chatId, UserRole role)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

            if (user is null)
            {
                _logger.LogWarning("User with ChatId {ChatId} not found", chatId);
                return Error.NotFound(description: "User not found");
            }

            if (user.Role == role)
            {
                _logger.LogInformation("User with ChatId {ChatId} already has role {Role}", chatId, role);
                return Error.Conflict(description: "User already has this role");
            }

            user.Role = role;
            _ = await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated role to {Role} for user with ChatId {ChatId}", role, chatId);
            return Result.Updated;
        }
    }
}