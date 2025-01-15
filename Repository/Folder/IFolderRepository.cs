namespace Trace.Repository.Folder;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models.Logic;

public interface IFolderRepository
{
    Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
    Task<Folder> GetFolderByIdAsync(Guid id, string userId);
    Task<Folder> CreateFolderAsync(Folder folder);
    Task<Folder> UpdateFolderAsync(Folder folder);
    Task<bool> DeleteFolderAsync(Guid id, string userId);
}

