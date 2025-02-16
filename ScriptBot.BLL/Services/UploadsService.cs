using ErrorOr;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ScriptBot.BLL.Interfaces;
using ScriptBot.DAL.Data;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Services
{
    public class UploadsService : IUploadsService
    {
        private readonly BotDbContext _dbContext;
        private readonly ILogger<UploadsService> _logger;

        public UploadsService(BotDbContext dbContext, ILogger<UploadsService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ErrorOr<Success>> AddUploadAsync(Upload upload)
        {
            _logger.LogInformation("Adding new upload for user {UserId}, app: {AppName}", upload.UserId, upload.AppName);

            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == upload.UserId);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when adding upload", upload.UserId);
                return Error.NotFound(description: "User not found");
            }

            await _dbContext.Uploads.AddAsync(upload);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully added upload {UploadId} for user {UserId}", upload.Id, upload.UserId);

            return Result.Success;
        }

        public async Task<IEnumerable<Upload>> GetLastUploadsAsync(int count)
        {
            _logger.LogInformation("Retrieving last {Count} uploads", count);

            List<Upload> uploads = await _dbContext.Uploads
                .Include(u => u.User)
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();

            _logger.LogInformation("Retrieved {ActualCount} uploads", uploads.Count);

            return uploads;
        }
    }
}