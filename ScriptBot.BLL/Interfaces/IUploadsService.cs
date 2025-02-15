using ErrorOr;

using ScriptBot.DAL.Entities;

namespace ScriptBot.BLL.Interfaces
{
    public interface IUploadsService
    {
        Task<IEnumerable<Upload>> GetLastUploadsAsync(int count);

        Task<ErrorOr<Success>> AddUploadAsync(Upload upload);
    }
}