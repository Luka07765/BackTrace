

namespace Trace.Repository.Files.Modify
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    using Trace.DTO;
    public class FileModifyRepository : IFileModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FileModifyRepository(ApplicationDbContext context, IConfiguration configuration)
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
        public async Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    var file = await _context.Files
                        .AsTracking()
                        .FirstOrDefaultAsync(f => f.Id == fileId);

                    if (file == null)
                        return null;

                    var rvProp = _context.Entry(file).Property(f => f.RowVersion);

                    if (input.RowVersion != null)
                    {
                     
                        rvProp.OriginalValue = input.RowVersion;
                    }
                    else
                    {
                      
                        rvProp.OriginalValue = file.RowVersion;
                        rvProp.IsModified = false;
                    }

                    var oldColor = file.Colors;
                    var oldFolderId = file.FolderId;

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
                        var newAncestors =
                            oldFolderId == newFolderId
                                ? oldAncestors
                                : await GetAncestorChainAsync(newFolderId);

                        void ApplyDelta(IEnumerable<Folder> folders, string color, int delta)
                        {
                            foreach (var folder in folders)
                            {
                                if (color == "Red")
                                    folder.RedCount = Math.Max(0, folder.RedCount + delta);
                                else if (color == "Yellow")
                                    folder.YellowCount = Math.Max(0, folder.YellowCount + delta);
                            }
                        }

                        if (oldColor is "Red" or "Yellow")
                            ApplyDelta(oldAncestors, oldColor, -1);

                        if (newColor is "Red" or "Yellow")
                            ApplyDelta(newAncestors, newColor, +1);
                    }

                    await _context.SaveChangesAsync();
                    return file;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (attempt == 1)
                        throw;

              
                    foreach (var entry in _context.ChangeTracker.Entries())
                        entry.State = EntityState.Detached;
                }

            }

            return null;
        }

  


        public async Task<File> CreateFileAsync(File file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<int> DeleteFileAsync(Guid id)
        {
            return await _context.Files
                .Where(f => f.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
