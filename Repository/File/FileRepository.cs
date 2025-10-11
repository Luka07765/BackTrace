namespace Trace.Repository.File
{
    using Microsoft.EntityFrameworkCore;
    using Dapper;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.Models.Logic;
    using Npgsql;
    using Microsoft.Extensions.Configuration;

    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;      // For EF Core operations
        private readonly string _connectionString;          // For Dapper operations

        public FileRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // EF Core: Get all files for a user
        public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
        {
            return await _context.Files
                .Where(f => f.UserId == userId)
                .Select(f => new File
                {
                    Id = f.Id,
                    Title = f.Title,
                    FolderId = f.FolderId,
                    FilePosition = f.FilePosition
                })
                .ToListAsync();
        }


        // EF Core: Get file by ID for a user
        public async Task<File> GetFileByIdAsync(Guid id, string userId)
        {
            return await _context.Files
                .Include(f => f.Folder)
                .Include(f => f.TagAssignments)        // load the join table
                    .ThenInclude(ta => ta.Tag)         // load the tag itself
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
        }

        // EF Core: Create a new file
        public async Task<File> CreateFileAsync(File file)
        {
            file.Id = Guid.NewGuid();
            file.Colors =  "Green" ;
            file.IconId = 1;    
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }
        public async Task<File?> SaveFileDeltaAsync(Guid fileId,
            Guid? folderId,
            string? colors,
            string? title,
            string? content,
            string userId,
            int? filePosition,
            int? iconId
            
            )
        {
            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);

            if (file == null) return null;

            if (title != null) file.Title = title;
            if (content != null) file.Content = content;
            if (colors != null) file.Colors = colors;
            if (filePosition.HasValue) file.FilePosition = filePosition.Value;
            if (iconId.HasValue) file.IconId = iconId.Value;
            if (folderId.HasValue)
            {
                // Verify the target folder exists and belongs to the user
                var folderExists = await _context.Folders
                    .AnyAsync(f => f.Id == folderId.Value && f.UserId == userId);

                if (!folderExists)
                {
                    throw new ArgumentException("Target folder doesn't exist or you don't have permission");
                }

                // Prevent moving to same folder
                if (file.FolderId == folderId.Value)
                {
                    throw new ArgumentException("File is already in this folder");
                }

                file.FolderId = folderId.Value;
            }



            await _context.SaveChangesAsync();

            return file;
        }

        public async Task<bool> DeleteFileAsync(Guid id, string userId)
        {
            var file = await GetFileByIdAsync(id, userId);
            if (file == null) return false;

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
