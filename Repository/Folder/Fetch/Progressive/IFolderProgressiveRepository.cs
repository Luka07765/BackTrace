namespace Trace.Repository.Folder.Fetch.Progressive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public interface IFolderProgressiveRepository
    {
        Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(
            Guid folderId, string userId, CancellationToken ct = default);

        Task<(List<Folder> SubFolders, List<File> Files)> GetChildrenAsync(
            IReadOnlyCollection<Guid> parentFolderIds, string userId, CancellationToken ct = default);
    }
}