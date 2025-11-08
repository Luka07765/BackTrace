
namespace Trace.Repository.Files.Fetch
{
    using Microsoft.EntityFrameworkCore;
    using Dapper;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;
    using Npgsql;
    using Microsoft.Extensions.Configuration;
    public class FileQueryRepository : IFileQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public FileQueryRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

        }


        public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Select(f => new File
                {
                    Id = f.Id,
                    Title = f.Title,
                    FolderId = f.FolderId,
                    FilePosition = f.FilePosition
                })
                .ToListAsync();
        }


     
        public async Task<File> GetFileByIdAsync(Guid id, string userId)
        {
            return await _context.Files
                .Include(f => f.TagAssignments)       
                .ThenInclude(ta => ta.Tag)     
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

    }
}
