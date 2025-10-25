
namespace Trace.Repository.Folder.Fetch.Progressive
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;

    public class FolderProgressiveRepository : IFolderProgressiveRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderProgressiveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Folder> GetFirstLayerAsync(Guid folderId, string userId)
        {
            return await _context.Folders
                .AsNoTracking()
                .Where(f => f.Id == folderId && f.UserId == userId)
                .Include(f => f.SubFolders)
                .Include(f => f.Files)
                .FirstOrDefaultAsync();
        }

    }
}
