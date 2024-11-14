namespace Trace.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;

public interface IFolderRepository
{
    Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
    Task<Folder> GetFolderByIdAsync(int id, string userId);
    Task<Folder> CreateFolderAsync(Folder folder);
    Task<Folder> UpdateFolderAsync(Folder folder);
    Task<bool> DeleteFolderAsync(int id, string userId);
}

