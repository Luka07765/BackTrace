namespace Trace.Service.Folder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.DTO;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    public interface IFolderService
    {

    
  
        Task<Folder> UpdateFolderAsync(Guid id, FolderInput input, string userId);
        //Task<bool> DeleteFolderAsync(Guid id, string userId);
 
    }
}

