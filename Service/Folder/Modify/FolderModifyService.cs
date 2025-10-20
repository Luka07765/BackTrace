

namespace Trace.Service.Folder.Modify
{
    using Trace.Data;
    using Trace.GraphQL.Inputs;
    using Trace.Repository.Folder.Modify;

    using System.Threading.Tasks;
  
    using Trace.Models.Logic;
    
    public class FolderModifyService : IFolderModifyService
    {
        private readonly IFolderModifyRepository _folderModifyRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FolderService> _logger;

        public FolderModifyService(IFolderModifyRepository folderModifyRepository, ApplicationDbContext context, ILogger<FolderService> logger)
        {
            _folderModifyRepository = folderModifyRepository;
            _context = context;
            _logger = logger;

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
            return await _folderModifyRepository.CreateFolderAsync(folder);
        }


    }
}
