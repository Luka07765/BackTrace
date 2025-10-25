


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
            var root = await _folderProgressiveRepository.GetFirstLayerAsync(rootFolderId, userId);
            if (root == null)
                yield break;

            int depth = 1;

        
            var currentLayer = new List<Folder> { root };

 
            while (currentLayer.Any() && !cancellationToken.IsCancellationRequested)
            {
          
                yield return new FolderLayerPayload(depth, currentLayer);

                var nextLayer = new List<Folder>();

        
                foreach (var folder in currentLayer)
                {
               
                    foreach (var sub in folder.SubFolders)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            yield break;

                        var fullSubFolder = await _folderProgressiveRepository.GetFirstLayerAsync(sub.Id, userId);
                        if (fullSubFolder != null)
                        {
                            sub.Files = fullSubFolder.Files;
                            sub.SubFolders = fullSubFolder.SubFolders;
                            nextLayer.Add(fullSubFolder);
                        }
                    }
                }
                if (nextLayer.Any())
                    await Task.Delay(5000, cancellationToken);
                currentLayer = nextLayer;
                depth++;
            }
        }


       
        }
    }

