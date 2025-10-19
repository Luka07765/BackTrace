namespace Trace.Repository.Folder.Fetch.Progressive
{

    using System.Threading.Tasks;
    using Trace.Models.Logic;
    public interface IFolderProgressiveRepository
    {
        Task<Folder> GetFirstLayerAsync(Guid folderId, string userId);
    }
}
