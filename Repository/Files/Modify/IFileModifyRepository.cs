namespace Trace.Repository.Files.Modify
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    public interface IFileModifyRepository
    {
        Task<File> CreateFileAsync(File file);
        Task<File?> UpdateFileAsync(Guid fileId, UpdateFileInput input);
  
        
        
        Task<bool> DeleteFileAsync(Guid id);
  
        Task<bool> SoftDeleteFileAsync(Guid fileId);
        Task<bool> RestoreFileAsync(Guid fileId);

    }
}
