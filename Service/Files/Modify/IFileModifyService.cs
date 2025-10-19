
namespace Trace.Service.Files.Modify
{
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;
    public interface IFileModifyService
    {
        Task<File> CreateFileAsync(CreateFileInput input, string userId);
    }
}
