

namespace Trace.Service.Folder.Fetch.Query
{
    using Trace.Data;
    using Trace.Models.Logic;
    using Trace.Repository.Folder.Fetch.Query;

    public class FolderQueryService : IFolderQueryService
    {
        private readonly IFolderQueryRepository _folderQueryRepository;


        public FolderQueryService(IFolderQueryRepository folderQueryRepository)
        {
            _folderQueryRepository = folderQueryRepository;
       

        }


        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId)
        {
            return await _folderQueryRepository.GetAllFoldersAsync(userId);
        }

        public async Task<Folder> GetFolderByIdAsync(Guid id, string userId)
        {
            return await _folderQueryRepository.GetFolderByIdAsync(id, userId);
        }

        public async Task<IEnumerable<Folder>> GetRootFoldersAsync(string userId)
        {
            return await _folderQueryRepository.GetRootFoldersAsync(userId);
        }
    }
}
