namespace Trace.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;
using Trace.GraphQL.Inputs;
public interface IFileService
{
    Task<IEnumerable<File>> GetAllFilesAsync(string userId);
    Task<File> GetFileByIdAsync(int id, string userId);
    Task<File> CreateFileAsync(FileInput input, string userId);
    Task<File> UpdateFileAsync(int id, FileInput input, string userId);
    Task<bool> DeleteFileAsync(int id, string userId);
}