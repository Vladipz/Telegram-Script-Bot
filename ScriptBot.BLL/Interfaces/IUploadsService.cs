using ErrorOr;

using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    /// <summary>
    /// Interface for managing file upload operations.
    /// </summary>
    public interface IUploadsService
    {
        /// <summary>
        /// Retrieves the specified number of most recent uploads.
        /// </summary>
        /// <param name="count">The number of uploads to retrieve.</param>
        /// <returns>A collection of the most recent uploads.</returns>
        Task<IEnumerable<Upload>> GetLastUploadsAsync(int count);

        /// <summary>
        /// Adds a new upload record to the system.
        /// </summary>
        /// <param name="upload">The upload entity to add.</param>
        /// <returns>A success result if the upload was added successfully, or an error if it failed.</returns>
        Task<ErrorOr<Success>> AddUploadAsync(Upload upload);
    }
}