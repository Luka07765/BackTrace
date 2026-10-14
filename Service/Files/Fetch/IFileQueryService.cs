namespace Trace.Service.Files.Fetch
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public interface IFileQueryService
    {
        Task<IEnumerable<File>> GetAllFilesAsync(string userId, CancellationToken ct = default);
        Task<File?> GetFileByIdAsync(Guid id, string userId, bool includeTrashed = false, CancellationToken ct = default);
        Task<List<File>> GetFileTrashedAsync(string userId, CancellationToken ct = default);
    }
}