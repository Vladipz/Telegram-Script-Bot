using ErrorOr;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Mappings;
using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly BotDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(BotDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ErrorOr<Created>> CreateUserAsync(CreateUserModel model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == model.ChatId);

            if (existingUser != null)
            {
                _logger.LogWarning("Attempted to create duplicate user with ChatId: {ChatId}", model.ChatId);
                return Error.Conflict(description: "User already exists");
            }

            var user = model.ToEntity();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new user with ChatId: {ChatId}", model.ChatId);
            return Result.Created;
        }

        public async Task<ErrorOr<User>> GetUserAsync(long chatId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ChatId: {ChatId}", chatId);
                return Error.NotFound(description: "User not found");
            }

            return user;
        }

        public async Task<UserRole> GetUserRoleAsync(long chatId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

            var role = user?.Role ?? UserRole.Guest;

            return role;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
    }
}