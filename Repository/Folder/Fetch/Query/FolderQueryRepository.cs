namespace Trace.Repository.Folder.Fetch.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FolderQueryRepository : IFolderQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId, CancellationToken ct = default)
        {
            var folders = await _context.Folders
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .ToListAsync(ct);

            // Link children to parents in memory instead of a second Include query.
            var childrenByParent = folders.ToLookup(f => f.ParentFolderId);
            foreach (var folder in folders)
            {
                folder.SubFolders = childrenByParent[folder.Id].ToList();
            }

            return folders;
        }

        public Task<Folder?> GetFolderByIdAsync(Guid id, string userId, CancellationToken ct = default)
        {
            return _context.Folders
                .AsNoTracking()
                .Where(f => f.Id == id && f.UserId == userId)
                .Include(f => f.SubFolders)
                .Include(f => f.Files.Where(file => file.DeletedAt == null))
                .AsSplitQuery()
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Folders
                .AsNoTracking()
                .Where(f => f.UserId == userId && f.ParentFolderId == null)
                .ToListAsync(ct);
        }
    }
}