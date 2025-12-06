

namespace Trace.Repository.Folder.Fetch.Query
{
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;
    using Trace.Repository.Folder.Fetch.Colors;

    public class FolderQueryRepository: IFolderQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly FolderColorsRepository _folderColorsRepository;
        public FolderQueryRepository(ApplicationDbContext context, FolderColorsRepository folderColorsRepository)
        {
            _context = context; 
            _folderColorsRepository = folderColorsRepository;
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
            var roots = await _context.Folders
                .Where(f => f.UserId == userId && f.ParentFolderId == null)
                .ToListAsync();

            foreach (var folder in roots)
            {
                var counts = await _folderColorsRepository.FolderColorsCount(folder.Id, userId);
                folder.ColorCounts = counts.ToList();
            }

            return roots;
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
