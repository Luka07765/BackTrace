
namespace Trace.Service.Folder.Modify
{
    using Trace.Models.Data;

    using Trace.GraphQL.Inputs;
    using System.Threading.Tasks;
    public interface IFolderModifyService
    {
        Task<Folder> CreateFolderAsync(FolderInput input, string userId);
        Task<Folder?> UpdateFolderAsync(Guid folderId, FolderInput input);
        Task<bool> DeleteFolderAsync(Guid id);


    }
}
