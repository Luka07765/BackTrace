
namespace Trace.Repository.Color
{
    using Trace.Models.Data;

    public interface IColorRepository
    {
        Task<List<Folder>> GetAncestorChainAsync(Guid folderId);
    }
}
