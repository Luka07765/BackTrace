namespace Trace.Repository.Files;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models.Logic;

public interface IFileRepository
{



    Task<File> SaveFileDeltaAsync(Guid id, Guid? folderId, string? colors, string? title, string? content, string userId, int? filePosition,int? iconId);
}
