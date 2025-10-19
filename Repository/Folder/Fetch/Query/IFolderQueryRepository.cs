namespace Trace.Repository.Folder.Fetch.Normal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    public interface IFolderQueryRepository
    {
        Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
        Task<Folder> GetFolderByIdAsync(Guid id, string userId);

        Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId);
    }
}
