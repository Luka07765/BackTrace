namespace Trace.Repository.File;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models.Logic;

public interface IFileRepository
{
    Task<IEnumerable<File>> GetAllFilesAsync(string userId);
    Task<File> GetFileByIdAsync(Guid id, string userId);
    Task<File> CreateFileAsync(File file);
    Task<bool> DeleteFileAsync(Guid id, string userId);
    Task<File> SaveFileDeltaAsync(Guid id, Guid? folderId, string? colors, string? title, string? content, string userId, int? filePosition,int? iconId);
}
