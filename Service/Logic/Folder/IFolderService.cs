namespace Trace.Service.Logic.Folder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public interface IFolderService
    {
        Task<bool> IsFolderOwnedByUserAsync(Guid folderId, string userId);
        Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
        Task<Folder> GetFolderByIdAsync(Guid id, string userId);
        Task<Folder> CreateFolderAsync(FolderInput input, string userId);
        Task<Folder> UpdateFolderAsync(Guid id, FolderInput input, string userId);
        Task<bool> DeleteFolderAsync(Guid id, string userId);
    }
}
