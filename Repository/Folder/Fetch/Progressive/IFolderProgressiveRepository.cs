namespace Trace.Repository.Folder.Fetch.Progressive
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public interface IFolderProgressiveRepository
    {
        Task<(List<Folder> SubFolders, List<File> Files)> GetContentsAsync(Guid folderId, string userId);
    }
}
