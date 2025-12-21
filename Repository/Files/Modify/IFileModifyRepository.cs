namespace Trace.Repository.Files.Modify
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    public interface IFileModifyRepository
    {
        Task<File> CreateFileAsync(File file);
        Task<bool> DeleteFileAsync(Guid id);
        Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input);
    }
}
