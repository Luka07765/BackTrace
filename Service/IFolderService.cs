namespace Trace.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;
using Trace.GraphQL.Inputs;
public interface IFolderService
{
    Task<IEnumerable<Folder>> GetAllFoldersAsync();
    Task<Folder> GetFolderByIdAsync(int id);
    Task<Folder> CreateFolderAsync(FolderInput input);
    Task<Folder> UpdateFolderAsync(int id, FolderInput input);
    Task<bool> DeleteFolderAsync(int id);
}
