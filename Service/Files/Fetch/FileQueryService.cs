namespace Trace.Service.Files.Fetch
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    using Trace.Repository.Files.Fetch;

    public class FileQueryService : IFileQueryService
    {
        private readonly IFileQueryRepository _fileQueryRepository;

        public FileQueryService(IFileQueryRepository fileQueryRepository)
        {
            _fileQueryRepository = fileQueryRepository;
        }

        public Task<IEnumerable<File>> GetAllFilesAsync(string userId, CancellationToken ct = default)
            => _fileQueryRepository.GetAllFilesAsync(userId, ct);

        public Task<File?> GetFileByIdAsync(Guid id, string userId, bool includeTrashed = false, CancellationToken ct = default)
            => _fileQueryRepository.GetFileByIdAsync(id, userId, includeTrashed, ct);

        public Task<List<File>> GetFileTrashedAsync(string userId, CancellationToken ct = default)
            => _fileQueryRepository.GetFileTrashedAsync(userId, ct);
    }
}