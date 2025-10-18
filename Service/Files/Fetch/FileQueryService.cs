
namespace Trace.Service.Files.Fetch
{
    using Trace.Repository.Files.Fetch;

    using Trace.Models.Logic;
    public class FileQueryService : IFileQueryService
    {
        private readonly IFileQueryRepository _fileQueryRepository;

        public FileQueryService(IFileQueryRepository fileRepository)
        {
            _fileQueryRepository = fileRepository;
        }

        public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
        {
            return await _fileQueryRepository.GetAllFilesAsync(userId);
        }

        public async Task<File> GetFileByIdAsync(Guid id, string userId)
        {
            return await _fileQueryRepository.GetFileByIdAsync(id, userId);
        }

    }
}
