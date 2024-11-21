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

        public async Task<Folder> GetFolderByIdAsync(int id, string userId)
        {
            return await _folderRepository.GetFolderByIdAsync(id, userId);
        }

        public async Task<Folder> CreateFolderAsync(FolderInput input, string userId)
        {
            var folder = new Folder
            {
                Title = input.Title,
                ParentFolderId = input.ParentFolderId,
                UserId = userId
            };
            return await _folderRepository.CreateFolderAsync(folder);
        }

        public async Task<Folder> UpdateFolderAsync(int id, FolderInput input, string userId)
        {
            var folder = await _folderRepository.GetFolderByIdAsync(id, userId);
            if (folder == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this folder.");
            }

            folder.Title = input.Title;
            folder.ParentFolderId = input.ParentFolderId;
            return await _folderRepository.UpdateFolderAsync(folder);
        }

        public async Task<bool> DeleteFolderAsync(int id, string userId)
        {
            var folder = await _folderRepository.GetFolderByIdAsync(id, userId);
            if (folder == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this folder.");
            }

            return await _folderRepository.DeleteFolderAsync(id, userId);
        }

        public async Task<bool> IsFolderOwnedByUserAsync(int folderId, string userId)
        {
            // Check folder ownership using the ApplicationDbContext
            return await _context.Folders.AnyAsync(f => f.Id == folderId && f.UserId == userId);
        }
    }
}
