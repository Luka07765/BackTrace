namespace Trace.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;
using Trace.GraphQL.Inputs;
public interface IFileService
{
    Task<IEnumerable<File>> GetAllFilesAsync();
    Task<File> GetFileByIdAsync(int id);
    Task<File> CreateFileAsync(FileInput input);
    Task<File> UpdateFileAsync(int id, FileInput input);
    Task<bool> DeleteFileAsync(int id);
}
