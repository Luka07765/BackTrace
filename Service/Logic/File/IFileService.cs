namespace Trace.Service.Logic.File;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.GraphQL.Inputs;
using Trace.Models.Logic;

public interface IFileService
{
   

    Task<File> UpdateFileAsync(Guid id, UpdateFileInput input, string userId);
    //Task<bool> DeleteFileAsync(Guid id);
}