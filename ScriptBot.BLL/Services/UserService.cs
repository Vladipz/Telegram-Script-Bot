using ErrorOr;

using Microsoft.EntityFrameworkCore;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly BotDbContext _dbContext;

        public UserService(BotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ErrorOr<Success>> AddUploadAsync(Upload upload)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == upload.UserId);

            if (user == null)
            {
                return Error.NotFound(description: "User not found");
            }

            await _dbContext.Uploads.AddAsync(upload);
            await _dbContext.SaveChangesAsync();

            return Result.Success;
        }

        public async Task<ErrorOr<Created>> CreateUserAsync(CreateUserModel model)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == model.ChatId);

            if (existingUser != null)
            {
                return Error.Conflict(description: "User already exists");
            }

            // TODO: add mapping
            var user = new User
            {
                ChatId = model.ChatId,
                Username = model.Username,
                Role = model.Role,
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Result.Created;
        }

        public async Task<ErrorOr<User>> GetUserAsync(long chatId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if (user == null)
            {
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