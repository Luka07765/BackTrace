namespace Trace.Repository.Files.Fetch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FileQueryRepository : IFileQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public FileQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<File>> GetAllFilesAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Files
                .AsNoTracking()
                .Where(f => f.UserId == userId && f.DeletedAt == null)
                .Include(f => f.TagAssignments)
                    .ThenInclude(ta => ta.Tag)
                .AsSplitQuery()
                .ToListAsync(ct);
        }

        public Task<File?> GetFileByIdAsync(Guid id, string userId, bool includeTrashed = false, CancellationToken ct = default)
        {
            return _context.Files
                .AsNoTracking()
                .Where(f => f.Id == id && f.UserId == userId)
                .Where(f => includeTrashed || f.DeletedAt == null)
                .Include(f => f.TagAssignments)
                    .ThenInclude(ta => ta.Tag)
                .AsSplitQuery()
                .FirstOrDefaultAsync(ct);
        }

        public Task<List<File>> GetFileTrashedAsync(string userId, CancellationToken ct = default)
        {
            return _context.Files
                .AsNoTracking()
                .Where(f => f.UserId == userId && f.DeletedAt != null)
                .OrderByDescending(f => f.DeletedAt)
                .ToListAsync(ct);
        }
    }
}