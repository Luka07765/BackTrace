namespace Trace.Service.Logic.Folder
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

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId)
        {
            return await _folderRepository.GetAllFoldersAsync(userId);
        }

        public async Task<Folder> GetFolderByIdAsync(Guid id, string userId)
        {
            return await _folderRepository.GetFolderByIdAsync(id, userId);
        }

        public async Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId)
        {
            return await _folderRepository.GetRootFoldersAsync(userId);
        }

        public async Task<Folder> GetFirstLayerAsync(Guid folderId, string userId)
        {
            return await _folderRepository.GetFirstLayerAsync(folderId, userId);
        }

        public async IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
    Guid rootFolderId,
    string userId,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int depth = 1;
            var currentLayer = await _folderRepository.GetFirstLayerAsync(rootFolderId, userId);
            if (currentLayer == null)
                yield break;

            // Send the first layer
            yield return new FolderLayerPayload(depth, new List<Folder> { currentLayer });

            await foreach (var layer in StreamSubFoldersRecursive(currentLayer, userId, depth + 1, cancellationToken))
            {
                yield return layer;
            }
        }

        private async IAsyncEnumerable<FolderLayerPayload> StreamSubFoldersRecursive(
            Folder folder,
            string userId,
            int depth,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var subFolder in folder.SubFolders)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var fullSubFolder = await _folderRepository.GetFirstLayerAsync(subFolder.Id, userId);
                if (fullSubFolder != null)
                {
                    subFolder.Files = fullSubFolder.Files;
                    subFolder.SubFolders = fullSubFolder.SubFolders;

                    yield return new FolderLayerPayload(depth, new List<Folder> { fullSubFolder });

                    await foreach (var nestedLayer in StreamSubFoldersRecursive(fullSubFolder, userId, depth + 1, cancellationToken))
                    {
                        yield return nestedLayer;
                    }
                }
            }
        }

        public async Task<Folder> GetFolderTreeAsync(Guid folderId, string userId)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Starting recursive fetch for folder {FolderId}", folderId);

            var root = await _folderRepository.GetFirstLayerAsync(folderId, userId);
            if (root == null)
            {
                _logger.LogWarning("Folder {FolderId} not found", folderId);
                return null;
            }

            await LoadSubFoldersRecursive(root, userId, 1);

            sw.Stop();
            _logger.LogInformation("Finished fetching full tree for {FolderId} in {Elapsed}ms", folderId, sw.ElapsedMilliseconds);
            return root;
        }

        private async Task LoadSubFoldersRecursive(Folder folder, string userId, int depth)
        {
            _logger.LogInformation("Fetching layer {Depth} for folder {FolderId}", depth, folder.Id);

            foreach (var subFolder in folder.SubFolders)
            {
                var sw = Stopwatch.StartNew();

                var fullSubFolder = await _folderRepository.GetFirstLayerAsync(subFolder.Id, userId);
                if (fullSubFolder != null)
                {
                    subFolder.Files = fullSubFolder.Files;
                    subFolder.SubFolders = fullSubFolder.SubFolders;

                    _logger.LogInformation(
                        "Loaded folder {FolderId} with {FileCount} files and {SubCount} subfolders at depth {Depth} (took {Elapsed}ms)",
                        subFolder.Id, subFolder.Files.Count, subFolder.SubFolders.Count, depth, sw.ElapsedMilliseconds
                    );

                    await LoadSubFoldersRecursive(subFolder, userId, depth + 1);
                }
            }
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
