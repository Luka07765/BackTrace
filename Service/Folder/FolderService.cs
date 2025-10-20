namespace Trace.Service.Folder
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.DTO;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    using Trace.Repository.Folder;

    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FolderService> _logger;

        public FolderService(IFolderRepository folderRepository, ApplicationDbContext context, ILogger<FolderService> logger)
        {
            _folderRepository = folderRepository;
            _context = context;
            _logger = logger;
    
        }





        //public async Task<bool> DeleteFolderAsync(Guid id, string userId)
        //{
        //    // Retrieve the folder to ensure the user has access
        //    var folder = await _folderRepository.GetFolderByIdAsync(id, userId);
        //    if (folder == null)
        //    {
        //        throw new UnauthorizedAccessException("You do not have permission to delete this folder.");
        //    }

        //    // Check for child folders or files
        //    var childFolders = await _context.Folders.Where(f => f.ParentFolderId == id).ToListAsync();
        //    var associatedFiles = await _context.Files.Where(f => f.FolderId == id).ToListAsync();

        //    // Delete associated files
        //    _context.Files.RemoveRange(associatedFiles);

        //    // Delete child folders recursively
        //    foreach (var childFolder in childFolders)
        //    {
        //        await DeleteFolderAsync(childFolder.Id, userId);
        //    }

        //    // Delete the folder
        //    _context.Folders.Remove(folder);

        //    // Persist changes to the database
        //    await _context.SaveChangesAsync();

        //    return true;
        //}



      
    }
}
