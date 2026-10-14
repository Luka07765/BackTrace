namespace Trace.Repository.Folder.Fetch.Progressive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FolderProgressiveRepository : IFolderProgressiveRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public FolderProgressiveRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(
            Guid folderId, string userId, CancellationToken ct = default)
            => GetChildrenAsync(new[] { folderId }, userId, ct);

        // All children of a whole layer: 2 queries on 1 connection, however many folders are in the layer.
        public async Task<(List<Folder> SubFolders, List<File> Files)> GetChildrenAsync(
            IReadOnlyCollection<Guid> parentFolderIds, string userId, CancellationToken ct = default)
        {
            if (parentFolderIds.Count == 0)
                return (new List<Folder>(), new List<File>());

            // Guid? so Contains works whether FolderId / ParentFolderId are nullable or not.
            var ids = parentFolderIds.Select(id => (Guid?)id).ToList();

            await using var context = await _contextFactory.CreateDbContextAsync(ct);

            var subFolders = await context.Folders
                .AsNoTracking()
                .Where(f => f.UserId == userId && ids.Contains(f.ParentFolderId))
                .ToListAsync(ct);

            var files = await context.Files
                .AsNoTracking()
                .Where(f => f.UserId == userId && f.DeletedAt == null && ids.Contains(f.FolderId))
                .ToListAsync(ct);

            return (subFolders, files);
        }
    }
}