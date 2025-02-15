using ErrorOr;

using Microsoft.EntityFrameworkCore;

using ScriptBot.BLL.Interfaces;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class UploadsService : IUploadsService
    {
        private readonly BotDbContext _dbContext;

        public UploadsService(BotDbContext dbContext)
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

        public async Task<IEnumerable<Upload>> GetLastUploadsAsync(int count)
        {
            var uploads = await _dbContext.Uploads
                .Include(u => u.User)
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();

            return uploads;
        }
    }
}