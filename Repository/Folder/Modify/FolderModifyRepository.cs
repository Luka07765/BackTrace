

namespace Trace.Repository.Folder.Modify
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    using Trace.Repository.Color;

    public class FolderModifyRepository : IFolderModifyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IColorRepository _colorRepository;
        public FolderModifyRepository(
        ApplicationDbContext context,
        IColorRepository colorRepository)
        {
            _context = context;
            _colorRepository = colorRepository;
        }



        public async Task<Folder> CreateFolderAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }
        public async Task<Folder?> UpdateFolderAsync(Guid folderId, FolderInput input)
        {
            var folder = await _context.Folders
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == folderId);

            if (folder == null) return null;

            var oldParentId = folder.ParentFolderId;

            if (input.Title != null)
                folder.Title = input.Title;

            if (input.IconId.HasValue)
                folder.IconId = input.IconId.Value;

            if (input.ParentFolderId.HasValue)
                folder.ParentFolderId = input.ParentFolderId.Value;

            var newParentId = folder.ParentFolderId;

          
            if (oldParentId != newParentId)
            {
          
                if (oldParentId.HasValue)
                {
                    var oldAncestors = await _colorRepository.GetAncestorChainAsync(oldParentId.Value);

                    foreach (var ancestor in oldAncestors)
                    {
                        ancestor.RedCount = Math.Max(0, ancestor.RedCount - folder.RedCount);
                        ancestor.YellowCount = Math.Max(0, ancestor.YellowCount - folder.YellowCount);
                    }
                }

                if (newParentId.HasValue)
                {
                    var newAncestors = await _colorRepository.GetAncestorChainAsync(newParentId.Value);

                    foreach (var ancestor in newAncestors)
                    {
                        ancestor.RedCount += folder.RedCount;
                        ancestor.YellowCount += folder.YellowCount;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return folder;
        }




    }
}
