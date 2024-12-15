namespace Trace.Repository.File;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models.Logic;

public interface IFileRepository
{
    Task<IEnumerable<File>> GetAllFilesAsync(string userId);
    Task<File> GetFileByIdAsync(int id, string userId);
    Task<File> CreateFileAsync(File file);
    Task<File> SaveFileDeltaAsync(int fileId, string? title, string? content, string userId);
    Task<bool> DeleteFileAsync(int id, string userId);
}
