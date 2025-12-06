using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.DTO;

namespace Trace.Repository.Folder.Fetch.Colors
{
    public class FolderColorsRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderColorsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ColorCountDto>> FolderColorsCount(Guid rootFolderId, string userId)
        {
            var sql = @"
    WITH RECURSIVE ""RecursiveFolders"" AS (
        SELECT ""Id""
        FROM ""Folders""
        WHERE ""Id"" = {0} AND ""UserId"" = {1}

        UNION ALL

        SELECT f.""Id""
        FROM ""Folders"" f
        INNER JOIN ""RecursiveFolders"" rf
            ON f.""ParentFolderId"" = rf.""Id""
        WHERE f.""UserId"" = {1}
    )
    SELECT ""Colors"" AS ""Color"", COUNT(*) AS ""Count""
    FROM ""Files""
    WHERE ""FolderId"" IN (SELECT ""Id"" FROM ""RecursiveFolders"")
    GROUP BY ""Colors"";
";



            return await _context.Set<ColorCountDto>()
                .FromSqlRaw(sql, rootFolderId, userId)
                .ToListAsync();
        }
    }
}
