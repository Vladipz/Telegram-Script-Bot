namespace ScriptBot.DAL.Entities
{
    /// <summary>
    /// Represents the different roles a user can have in the system.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// A guest user with limited access.
        /// </summary>
        Guest,

        /// <summary>
        /// A regular user with standard access.
        /// </summary>
        User,

        /// <summary>
        /// An administrator with full access.
        /// </summary>
        Admin,
    }
}