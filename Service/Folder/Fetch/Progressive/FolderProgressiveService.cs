


namespace Trace.Service.Folder.Fetch.Progressive
{

    using System.Collections.Generic;

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
                await Task.Delay(1000, cancellationToken); //fetcg every 1 secound
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
    }
}
