namespace Trace.Repository.Folder.Modify
{
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    using Trace.GraphQL.Inputs; 

    public interface IFolderModifyRepository
    {
        Task<Folder?> UpdateFolderAsync(Guid folderId, FolderInput input);
        Task<Folder> CreateFolderAsync(Folder folder);
    }
}
