

namespace Trace.Repository.Folder.Fetch.Query
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;
    public class FolderQueryRepository: IFolderQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId)
        {
            return await _context.Folders
          .Where(f => f.UserId == userId)
          .Include(f => f.SubFolders)

          .ToListAsync();
        }

        public async Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId)
        {
            return await _context.Folders
                .Where(f => f.UserId == userId && f.ParentFolderId == null)

                .ToListAsync();
        }


        public async Task<Folder> GetFolderByIdAsync(Guid id, string userId)
        {
            return await _context.Folders
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

    }
}
