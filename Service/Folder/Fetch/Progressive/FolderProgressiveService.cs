namespace Trace.Service.Folder.Fetch.Progressive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Trace.DTO;
    using Trace.Models.Logic;
    using Trace.Repository.Folder.Fetch.Progressive;

    public class FolderProgressiveService : IFolderProgressiveService
    {
        // Hard stop in case the data is broken in a way the cycle guard can't catch.
        private const int MaxDepth = 64;

        private readonly IFolderProgressiveRepository _repo;
        private readonly ILogger<FolderProgressiveService> _logger;

        public FolderProgressiveService(
            IFolderProgressiveRepository repo,
            ILogger<FolderProgressiveService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(
            Guid folderId, string userId, CancellationToken cancellationToken = default)
            => _repo.GetContentsAsync(folderId, userId, cancellationToken);

        // Breadth-first: layer 1 = direct children of the root, layer 2 = their children, ...
        // Each layer is sent to the client as soon as it's fetched (one repository call per layer).
        public async IAsyncEnumerable<FolderLayerPayload> StreamFolderHierarchyAsync(
            Guid rootFolderId,
            string userId,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var visited = new HashSet<Guid> { rootFolderId };
            var currentIds = new List<Guid> { rootFolderId };
            var depth = 0;

            while (currentIds.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (depth >= MaxDepth)
                {
                    _logger.LogWarning(
                        "Folder hierarchy for {RootFolderId} exceeded max depth {MaxDepth}; stopping stream.",
                        rootFolderId, MaxDepth);
                    yield break;
                }

                var (subFolders, files) = await _repo.GetChildrenAsync(currentIds, userId, cancellationToken);

                // Cycle guard: skip any folder we've already sent.
                var newFolders = new List<Folder>(subFolders.Count);
                foreach (var folder in subFolders)
                {
                    if (visited.Add(folder.Id))
                        newFolders.Add(folder);
                    else
                        _logger.LogWarning(
                            "Cycle in folder tree of {RootFolderId}: folder {FolderId} seen twice.",
                            rootFolderId, folder.Id);
                }

                if (newFolders.Count == 0 && files.Count == 0)
                    yield break;

                depth++;
                yield return new FolderLayerPayload(depth, newFolders, files);

                currentIds = newFolders.Select(f => f.Id).ToList();
            }
        }
    }
}