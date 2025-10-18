namespace Trace.Repository.Files.Fetch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    public interface IFileQueryRepository
    {
        Task<IEnumerable<File>> GetAllFilesAsync(string userId);
        Task<File> GetFileByIdAsync(Guid id, string userId);
    }
}
