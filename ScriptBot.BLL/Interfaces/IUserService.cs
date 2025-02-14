using ErrorOr;

using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<ErrorOr<Created>> CreateUserAsync(CreateUserModel model);

        Task<UserRole> GetUserRoleAsync(long chatId);
    }
}