

namespace Trace.Repository.Files.Modify
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public class FileModifyRepository : IFileModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FileModifyRepository(ApplicationDbContext context)
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

        private static void ApplyDelta(IEnumerable<Folder> folders, string color, int delta)
        {
            foreach (var folder in folders)
            {
                if (color == "Red")
                    folder.RedCount = Math.Max(0, folder.RedCount + delta);

                else if (color == "Yellow")
                    folder.YellowCount = Math.Max(0, folder.YellowCount + delta);
            }
        }

        public async Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var file = await _context.Files
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (file == null)
                return null;

            var oldColor = file.Colors;
            var oldFolderId = file.FolderId;

            // apply changes
            if (input.Title != null) file.Title = input.Title;
            if (input.Content != null) file.Content = input.Content;
            if (input.Colors != null) file.Colors = input.Colors;
            if (input.FilePosition.HasValue) file.FilePosition = input.FilePosition.Value;
            if (input.IconId.HasValue) file.IconId = input.IconId.Value;
            if (input.FolderId.HasValue) file.FolderId = input.FolderId.Value;

            var newColor = file.Colors;
            var newFolderId = file.FolderId;

            bool colorChanged = oldColor != newColor;
            bool folderChanged = oldFolderId != newFolderId;

            if (colorChanged || folderChanged)
            {
                var oldAncestors = await GetAncestorChainAsync(oldFolderId);
                var newAncestors = oldFolderId == newFolderId
                    ? oldAncestors
                    : await GetAncestorChainAsync(newFolderId);

                ApplyDelta(oldAncestors, oldColor, -1);
                ApplyDelta(newAncestors, newColor, +1);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return file;
        }

   
        public async Task<File> CreateFileAsync(File file)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            _context.Files.Add(file);

            if (file.Colors == "Red" || file.Colors == "Yellow")
            {
                var ancestors = await GetAncestorChainAsync(file.FolderId);
                ApplyDelta(ancestors, file.Colors, +1);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return file;
        }


        public async Task<int> DeleteFileAsync(Guid fileId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            var file = await _context.Files
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (file == null)
                return 0;

            if (file.Colors == "Red" || file.Colors == "Yellow")
            {
                var ancestors = await GetAncestorChainAsync(file.FolderId);
                ApplyDelta(ancestors, file.Colors, -1);
            }

            _context.Files.Remove(file);

            var result = await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return result;
        }
    }
}
