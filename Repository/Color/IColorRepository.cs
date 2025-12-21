
namespace Trace.Repository.Color
{
    using Trace.Models.Logic;
    public interface IColorRepository
    {
        Task<List<Folder>> GetAncestorChainAsync(Guid folderId);
    }
}
