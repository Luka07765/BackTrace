

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
        private readonly ILogger<FolderModifyService> _logger;

        public FolderModifyService(IFolderModifyRepository folderModifyRepository, ApplicationDbContext context, ILogger<FolderModifyService> logger)
        {
            _folderModifyRepository = folderModifyRepository;
            _context = context;
            _logger = logger;

        }




        public async Task<Folder> CreateFolderAsync(FolderInput input, string userId)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                throw new ArgumentException("Mora da ima naziv foldera.");

            var folder = new Folder
            {
                Title = input.Title,
                ParentFolderId = input.ParentFolderId,
                UserId = userId,
                IconId = input.IconId ?? 1
            };
            return await _folderModifyRepository.CreateFolderAsync(folder);
        }
       public async Task<Folder?> UpdateFolderAsync(Guid id, FolderInput input)
        {
            return await _folderModifyRepository.UpdateFolderAsync(id, input);
        }


    }
}
