namespace Trace.Repository.Folder.Modify
{
    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public interface IFolderModifyRepository
    {
        Task<Folder> CreateFolderAsync(Folder folder);
    }
}
