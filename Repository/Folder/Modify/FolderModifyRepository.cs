

namespace Trace.Repository.Folder.Modify
{
    using Trace.Models.Logic;
    using Trace.Data;
    public class FolderModifyRepository : IFolderModifyRepository
    {
        private readonly ApplicationDbContext _context;

        public FolderModifyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Folder> CreateFolderAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }


    }
}
