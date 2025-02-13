using ErrorOr;

using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    public interface IRoleManagerService
    {
        Task<ErrorOr<Updated>> UpdateUserRoleAsync(Guid userId, UserRole role);

        Task<ErrorOr<Updated>> UpdateUserRoleAsync(long chatId, UserRole role);

        Task<ErrorOr<UserRole>> GetUserRoleAsync(long chatId);

        Task<ErrorOr<IEnumerable<User>>> GetUsersByRoleAsync(UserRole role);
    }
}