

namespace Trace.Service.Folder.Fetch.Progressive
{
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.DTO;
    using Trace.Models.Logic;

    using Trace.Repository.Folder.Fetch.Progressive;

    public class FolderProgressiveService : IFolderProgressiveService
    {
        private readonly IFolderProgressiveRepository _repo;
        private readonly ILogger<FolderProgressiveService> _logger;
       

        public FolderProgressiveService(
            IFolderProgressiveRepository repo,
           
            ILogger<FolderProgressiveService> logger)
        {
            _repo = repo;
            _logger = logger;
     
        }

        public Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(Guid folderId, string userId)
            => _repo.GetContentsAsync(folderId, userId);

        public async IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
            Guid rootFolderId,
            string userId,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Fetch children of root (no root folder itself)
            var (subFolders, files) = await _repo.GetContentsAsync(rootFolderId, userId);
            subFolders = subFolders.Where(f => f.Id != rootFolderId).ToList();


            if (!subFolders.Any() && !files.Any())
                yield break;

            int depth = 1;
            yield return new FolderLayerPayload(depth, subFolders, files);

            var currentLayer = subFolders;

            // Iterate safely (strictly sequential, no concurrent DB calls)
            while (currentLayer.Any() && !cancellationToken.IsCancellationRequested)
            {
                var nextLayer = new List<Folder>();
                var nextLayerFiles = new List<File>();

                foreach (var folder in currentLayer)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    // 👇 Sequentially await each repository call
                    var contents = await _repo.GetContentsAsync(folder.Id, userId);
                    var childSubFolders = contents.SubFolders;
                    var childFiles = contents.Files;

                    childSubFolders = childSubFolders.Where(f => f.Id != rootFolderId).ToList();

                    folder.SubFolders = childSubFolders;
                    folder.Files = childFiles;

                    nextLayer.AddRange(childSubFolders);
                    nextLayerFiles.AddRange(childFiles);
                }

                if (!nextLayer.Any() && !nextLayerFiles.Any())
                    yield break;

                depth++;
                yield return new FolderLayerPayload(depth, nextLayer, nextLayerFiles);

                //await Task.Delay(5000, cancellationToken);

                currentLayer = nextLayer;
            }
        }
    }
}
