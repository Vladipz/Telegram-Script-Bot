using ErrorOr;

using ScriptBot.BLL.Models.User;
using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for user-related operations in the system.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their chat ID asynchronously.
        /// </summary>
        /// <param name="chatId">The chat ID of the user.</param>
        /// <returns>An <see cref="ErrorOr{T}"/> containing the user if found, or an error.</returns>
        Task<ErrorOr<User>> GetUserAsync(long chatId);

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="model">The model containing user creation details.</param>
        /// <returns>An <see cref="ErrorOr{T}"/> containing the result of the creation operation.</returns>
        Task<ErrorOr<Created>> CreateUserAsync(CreateUserModel model);

        /// <summary>
        /// Retrieves the role of a user by their chat ID asynchronously.
        /// </summary>
        /// <param name="chatId">The chat ID of the user.</param>
        /// <returns>The user's role if found.</returns>
        Task<UserRole> GetUserRoleAsync(long chatId);
    }
}