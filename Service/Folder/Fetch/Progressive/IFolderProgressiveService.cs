namespace Trace.Service.Folder.Fetch.Progressive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.DTO;
    using Trace.Models.Logic;

    public interface IFolderProgressiveService
    {
        Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(Guid folderId, string userId);

        IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
            Guid rootFolderId,
            string userId,
            CancellationToken cancellationToken = default);
    }
}


