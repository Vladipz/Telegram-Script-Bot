using ErrorOr;

using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    public interface IUserService
    {
        // TODO: move to separate interface
        Task<ErrorOr<Success>> AddUploadAsync(Upload upload);

        Task<ErrorOr<User>> GetUserAsync(long chatId);

        Task<IEnumerable<User>> GetUsersAsync();

        Task<ErrorOr<Created>> CreateUserAsync(CreateUserModel model);

        Task<UserRole> GetUserRoleAsync(long chatId);
    }
}