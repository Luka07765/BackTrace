

namespace Trace.Repository.Folder.Modify
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

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
        public async Task<Folder?> UpdateFolderAsync(Guid folderId, FolderInput input)
        {
            var folder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
            if (folder == null) return null;

            if (input.Title != null) folder.Title = input.Title;
            if (input.ParentFolderId.HasValue) folder.ParentFolderId = input.ParentFolderId.Value;
            if (input.IconId.HasValue) folder.IconId = input.IconId.Value;

            await _context.SaveChangesAsync();
            return folder;
        }


 
    }
}
