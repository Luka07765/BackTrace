


namespace Trace.Repository.Color
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    public class ColorRepository : IColorRepository
    {
        private readonly ApplicationDbContext _context;

        public ColorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Folder>> GetAncestorChainAsync(Guid folderId)
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

    }
}
