

namespace Trace.Service.Folder.Modify
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.GraphQL.Inputs;

    using Trace.Models.Logic;
    using Trace.Repository.Files.Modify;
    using Trace.Repository.Folder.Modify;

    public class FolderModifyService : IFolderModifyService
    {
        private readonly IFolderModifyRepository _folderModifyRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FolderModifyService> _logger;
        private readonly IFileModifyRepository _fileModifyRepository;
        public FolderModifyService(IFolderModifyRepository folderModifyRepository, IFileModifyRepository fileModifyRepository, ApplicationDbContext context, ILogger<FolderModifyService> logger)
        {
            _folderModifyRepository = folderModifyRepository;
            _context = context;
            _logger = logger;
            _fileModifyRepository = fileModifyRepository;

        }




        public async Task<Folder> CreateFolderAsync(FolderInput input, string userId)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                throw new ArgumentException("Mora da ima naziv foldera.");

            var folder = new Folder
            {
                Title = input.Title,
                ParentFolderId = input.ParentFolderId,
                UserId = userId,
                IconId = input.IconId ?? 1
            };
            return await _folderModifyRepository.CreateFolderAsync(folder);
        }
        public async Task<Folder?> UpdateFolderAsync(Guid id, FolderInput input)
        {
            return await _folderModifyRepository.UpdateFolderAsync(id, input);
        }
        public async Task<bool> DeleteFolderAsync(Guid id)
        {
            // 1️⃣ Delete files in this folder
            var associatedFiles = await _context.Files
                .Where(f => f.FolderId == id)
                .ToListAsync();

            if (associatedFiles.Any())
                _context.Files.RemoveRange(associatedFiles);

            // 2️⃣ Recursively delete child folders
            var childFolders = await _context.Folders
                .Where(f => f.ParentFolderId == id)
                .ToListAsync();

            foreach (var childFolder in childFolders)
                await DeleteFolderAsync(childFolder.Id);

            // 3️⃣ Delete this folder (use tracked entity if present)
            var folder = await _context.Folders.FindAsync(id);
            if (folder != null)
                _context.Folders.Remove(folder);

            await _context.SaveChangesAsync();
            return true;
        }

    }
}