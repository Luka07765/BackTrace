

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


        private async Task<List<Folder>> GetAncestorChainAsync(Guid folderId)
        {
            var sql = @"
        WITH RECURSIVE ancestors AS (
            SELECT * FROM ""Folders"" WHERE ""Id"" = {0}
            UNION ALL
            SELECT f.*
            FROM ""Folders"" f
            INNER JOIN ancestors a ON f.""Id"" = a.""ParentFolderId""
        )
        SELECT * FROM ancestors;
    ";

            return await _context.Folders
                .FromSqlRaw(sql, folderId)
                .AsTracking()
                .ToListAsync();
        }

        public async Task<Folder> CreateFolderAsync(FolderInput input, string userId)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                throw new ArgumentException("Mora da ima naziv foldera.");

            bool isRoot = input.ParentFolderId == null;

            if (isRoot && input.DomainId == null)
                throw new ArgumentException("Root folder must belong to a domain.");

            if (!isRoot && input.DomainId != null)
                throw new ArgumentException("Sub folder must not have domain.");

            var folder = new Folder
            {
                Id = input.Id.HasValue && input.Id.Value != Guid.Empty
                    ? input.Id.Value
                    : Guid.NewGuid(),

                Title = input.Title,
                DomainId = isRoot ? input.DomainId : null,
                ParentFolderId = isRoot ? null : input.ParentFolderId,
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
            // 1️⃣ Load folder with counts
            var folder = await _context.Folders
                .AsTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (folder == null)
                return false;

            // 2️⃣ Subtract aggregated counts from ancestors ONCE
            if (folder.ParentFolderId.HasValue)
            {
                var ancestors = await GetAncestorChainAsync(folder.ParentFolderId.Value);
                foreach (var ancestor in ancestors)
                {
                    ancestor.RedCount = Math.Max(0, ancestor.RedCount - folder.RedCount);
                    ancestor.YellowCount = Math.Max(0, ancestor.YellowCount - folder.YellowCount);
                }
            }

            // 3️⃣ Delete files in this folder
            var files = await _context.Files
                .Where(f => f.FolderId == id)
                .ToListAsync();

            if (files.Any())
                _context.Files.RemoveRange(files);

            // 4️⃣ Recursively delete child folders (NO COUNT LOGIC HERE)
            var childFolders = await _context.Folders
                .Where(f => f.ParentFolderId == id)
                .Select(f => f.Id)
                .ToListAsync();

            foreach (var childId in childFolders)
                await DeleteFolderAsync(childId);

            // 5️⃣ Delete this folder
            _context.Folders.Remove(folder);

            await _context.SaveChangesAsync();
            return true;
        }


     

    }
}