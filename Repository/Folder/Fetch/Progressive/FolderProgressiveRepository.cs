namespace Trace.Repository.Folder.Fetch.Progressive
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FolderProgressiveRepository : IFolderProgressiveRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public FolderProgressiveRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(Guid folderId, string userId)
        {
            // each query gets its own context instance
            await using var context1 = _contextFactory.CreateDbContext();
            await using var context2 = _contextFactory.CreateDbContext();

            var subFoldersTask = context1.Folders
                .AsNoTracking()
                .Where(f => f.ParentFolderId == folderId && f.UserId == userId)
                .ToListAsync();

            var filesTask = context2.Files
                .AsNoTracking()
                .Where(file => file.FolderId == folderId && file.UserId == userId)
                .ToListAsync();

            await Task.WhenAll(subFoldersTask, filesTask);

            return (await subFoldersTask, await filesTask);
        }

    }

}
