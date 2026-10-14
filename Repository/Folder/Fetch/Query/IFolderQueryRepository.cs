namespace Trace.Repository.Folder.Fetch.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public interface IFolderQueryRepository
    {
        Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId, CancellationToken ct = default);
        Task<Folder?> GetFolderByIdAsync(Guid id, string userId, CancellationToken ct = default);
        Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId, CancellationToken ct = default);
    }
}