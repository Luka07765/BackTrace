namespace Trace.Repository.Files.Modify
{
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    public interface IFileModifyRepository
    {
        Task<File> CreateFileAsync(File file);
    }
}
