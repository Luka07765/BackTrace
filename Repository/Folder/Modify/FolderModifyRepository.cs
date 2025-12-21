

namespace Trace.Repository.Folder.Modify
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public class FolderModifyRepository : IFolderModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderModifyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<List<Folder>> GetAncestorChainAsync(Guid folderId)
        {
            var sql = @"
        WITH RECURSIVE ancestors AS (
            SELECT * FROM ""Folders"" WHERE ""Id"" = {0}
            UNION ALL
            SELECT f.*
            FROM ""Folders"" f
            INNER JOIN ancestors a ON f.""Id"" = a.""ParentFolderId""
        )
        SELECT * FROM ancestors;
    ";

            return await _context.Folders
                .FromSqlRaw(sql, folderId)
                .AsTracking()
                .ToListAsync();
        }

        public async Task<Folder> CreateFolderAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }
        public async Task<Folder?> UpdateFolderAsync(Guid folderId, FolderInput input)
        {
            var folder = await _context.Folders
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == folderId);

            if (folder == null) return null;

            var oldParentId = folder.ParentFolderId;

            if (input.Title != null)
                folder.Title = input.Title;

            if (input.IconId.HasValue)
                folder.IconId = input.IconId.Value;

            if (input.ParentFolderId.HasValue)
                folder.ParentFolderId = input.ParentFolderId.Value;

            var newParentId = folder.ParentFolderId;

          
            if (oldParentId != newParentId)
            {
          
                if (oldParentId.HasValue)
                {
                    var oldAncestors = await GetAncestorChainAsync(oldParentId.Value);
                    foreach (var ancestor in oldAncestors)
                    {
                        ancestor.RedCount = Math.Max(0, ancestor.RedCount - folder.RedCount);
                        ancestor.YellowCount = Math.Max(0, ancestor.YellowCount - folder.YellowCount);
                    }
                }

                if (newParentId.HasValue)
                {
                    var newAncestors = await GetAncestorChainAsync(newParentId.Value);
                    foreach (var ancestor in newAncestors)
                    {
                        ancestor.RedCount += folder.RedCount;
                        ancestor.YellowCount += folder.YellowCount;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return folder;
        }




    }
}
