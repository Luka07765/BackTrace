namespace Trace.Service.Folder.Fetch.Progressive
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.DTO;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    public interface IFolderProgressiveService
    {
        Task<Folder> GetFolderTreeAsync(Guid folderId, string userId);
        Task<Folder> GetFirstLayerAsync(Guid folderId, string userId);

        IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
        Guid rootFolderId,
        string userId,
        CancellationToken cancellationToken = default);
    }
}


