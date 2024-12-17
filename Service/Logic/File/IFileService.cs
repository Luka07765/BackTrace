namespace Trace.Service.Logic.File;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.GraphQL.Inputs;
using Trace.Models.Logic;

public interface IFileService
{
   
    Task<IEnumerable<File>> GetAllFilesAsync(string userId);
    Task<File> GetFileByIdAsync(int id, string userId);
    Task<File> CreateFileAsync(CreateFileInput input, string userId);
    Task<File> UpdateFileAsync(int id, UpdateFileInput input, string userId);
    Task<bool> DeleteFileAsync(int id, string userId);
}