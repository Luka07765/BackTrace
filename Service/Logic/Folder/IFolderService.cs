namespace Trace.Service.Logic.Folder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public interface IFolderService
    {
        Task<bool> IsFolderOwnedByUserAsync(int folderId, string userId);
        Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
        Task<Folder> GetFolderByIdAsync(int id, string userId);
        Task<Folder> CreateFolderAsync(FolderInput input, string userId);
        Task<Folder> UpdateFolderAsync(int id, FolderInput input, string userId);
        Task<bool> DeleteFolderAsync(int id, string userId);
    }
}
