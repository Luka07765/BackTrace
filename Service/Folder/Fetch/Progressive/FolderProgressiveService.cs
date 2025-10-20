


namespace Trace.Service.Folder.Fetch.Progressive
{

    using System.Collections.Generic;

    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Trace.Data;
    using Trace.DTO;

    using Trace.Models.Logic;
    using Trace.Repository.Folder.Fetch.Progressive;
    public class FolderProgressiveService : IFolderProgressiveService
    {
        private readonly IFolderProgressiveRepository _folderProgressiveRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FolderProgressiveService> _logger;

        public FolderProgressiveService(IFolderProgressiveRepository folderProgressiveRepository, ApplicationDbContext context, ILogger<FolderProgressiveService> logger)
        {
            _folderProgressiveRepository = folderProgressiveRepository;
            _context = context;
            _logger = logger;

        }
        public async Task<Folder> GetFirstLayerAsync(Guid folderId, string userId)
        {
            return await _folderProgressiveRepository.GetFirstLayerAsync(folderId, userId);
        }

        public async IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
    Guid rootFolderId,
    string userId,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int depth = 1;
            var currentLayer = await _folderProgressiveRepository.GetFirstLayerAsync(rootFolderId, userId);
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

                var fullSubFolder = await _folderProgressiveRepository.GetFirstLayerAsync(subFolder.Id, userId);
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

            var root = await _folderProgressiveRepository.GetFirstLayerAsync(folderId, userId);
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

                var fullSubFolder = await _folderProgressiveRepository.GetFirstLayerAsync(subFolder.Id, userId);
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

    }
}
