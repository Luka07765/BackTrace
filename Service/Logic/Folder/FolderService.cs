namespace Trace.Service.Logic.Folder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    using Trace.Repository.Folder;

    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly ApplicationDbContext _context;

        public FolderService(IFolderRepository folderRepository, ApplicationDbContext context)
        {
            _folderRepository = folderRepository;
            _context = context;
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId)
        {
            return await _folderRepository.GetAllFoldersAsync(userId);
        }

        public async Task<Folder> GetFolderByIdAsync(Guid id, string userId)
        {
            return await _folderRepository.GetFolderByIdAsync(id, userId);
        }

        public async Task<Folder> CreateFolderAsync(FolderInput input, string userId)
        {
            var folder = new Folder
            {
                Title = input.Title,
                ParentFolderId = input.ParentFolderId,
                UserId = userId,
                IconId = input.IconId ?? 1
            };
            return await _folderRepository.CreateFolderAsync(folder);
        }

        public async Task<Folder> UpdateFolderAsync(Guid id, FolderInput input, string userId)
        {
            // Retrieve the existing folder
            var existingFolder = await _context.Folders.FindAsync(id);
            if (existingFolder == null)
            {
                throw new Exception("Folder not found");
            }

            // Check if the folder belongs to the user (optional, for security)
            if (existingFolder.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this folder.");
            }

            // Update only the properties that are provided
            if (input.Title != null)
            {
                existingFolder.Title = input.Title;
            }

            // Only update ParentFolderId if it's provided in the input
            if (input.ParentFolderId.HasValue)
            {
                existingFolder.ParentFolderId = input.ParentFolderId;
            }
            if (input.IconId.HasValue)
            {
                existingFolder.IconId = input.IconId.Value;
            }


            // Save changes
            await _context.SaveChangesAsync();
            return existingFolder;
        }


        public async Task<bool> DeleteFolderAsync(Guid id, string userId)
        {
            // Retrieve the folder to ensure the user has access
            var folder = await _folderRepository.GetFolderByIdAsync(id, userId);
            if (folder == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this folder.");
            }

            // Check for child folders or files
            var childFolders = await _context.Folders.Where(f => f.ParentFolderId == id).ToListAsync();
            var associatedFiles = await _context.Files.Where(f => f.FolderId == id).ToListAsync();

            // Delete associated files
            _context.Files.RemoveRange(associatedFiles);

            // Delete child folders recursively
            foreach (var childFolder in childFolders)
            {
                await DeleteFolderAsync(childFolder.Id, userId);
            }

            // Delete the folder
            _context.Folders.Remove(folder);

            // Persist changes to the database
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> IsFolderOwnedByUserAsync(Guid folderId, string userId)
        {
            // Check folder ownership using the ApplicationDbContext
            return await _context.Folders.AnyAsync(f => f.Id == folderId && f.UserId == userId);
        }
    }
}
