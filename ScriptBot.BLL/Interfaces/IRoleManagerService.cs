using ErrorOr;

using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for managing user roles in the system.
    /// </summary>
    public interface IRoleManagerService
    {
        /// <summary>
        /// Updates a user's role by their user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="role">The new role to assign.</param>
        /// <returns>A success result if updated, or a not found error.</returns>
        Task<ErrorOr<Updated>> UpdateUserRoleAsync(Guid userId, UserRole role);

        /// <summary>
        /// Updates a user's role by their chat ID.
        /// </summary>
        /// <param name="chatId">The chat ID of the user.</param>
        /// <param name="role">The new role to assign.</param>
        /// <returns>A success result if updated, or a not found error if user doesn't exist.</returns>
        Task<ErrorOr<Updated>> UpdateUserRoleAsync(long chatId, UserRole role);

        /// <summary>
        /// Gets the role of a user by their chat ID.
        /// </summary>
        /// <param name="chatId">The chat ID of the user.</param>
        /// <returns>The user's role if found, or a not found error.</returns>
        Task<ErrorOr<UserRole>> GetUserRoleAsync(long chatId);

        /// <summary>
        /// Gets all users with a specific role.
        /// </summary>
        /// <param name="role">The role to filter users by.</param>
        /// <returns>A collection of users with the specified role.</returns>
        Task<ErrorOr<IEnumerable<User>>> GetUsersByRoleAsync(UserRole role);
    }
}