namespace Trace.Service.Files.Fetch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Trace.Models.Logic;
    public interface IFileQueryService
    {
        Task<IEnumerable<File>> GetAllFilesAsync(string userId);
        Task<File> GetFileByIdAsync(Guid id, string userId);

        Task<List<File>> GetFileTrashedAsync(string userId);
    }
}
