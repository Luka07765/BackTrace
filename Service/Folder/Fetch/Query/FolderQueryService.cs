namespace Trace.Service.Folder.Fetch.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    using Trace.Repository.Folder.Fetch.Query;

    public class FolderQueryService : IFolderQueryService
    {
        private readonly IFolderQueryRepository _folderQueryRepository;

        public FolderQueryService(IFolderQueryRepository folderQueryRepository)
        {
            _folderQueryRepository = folderQueryRepository;
        }

        public Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId, CancellationToken ct = default)
            => _folderQueryRepository.GetAllFoldersAsync(userId, ct);

        public Task<Folder?> GetFolderByIdAsync(Guid id, string userId, CancellationToken ct = default)
            => _folderQueryRepository.GetFolderByIdAsync(id, userId, ct);

        public Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId, CancellationToken ct = default)
            => _folderQueryRepository.GetRootFoldersAsync(userId, ct);
    }
}