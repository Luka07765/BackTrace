namespace Trace.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;

public interface IFolderRepository
{
    Task<IEnumerable<Folder>> GetAllAsync();
    Task<Folder> GetByIdAsync(int id);
    Task<Folder> CreateAsync(Folder folder);
    Task<Folder> UpdateAsync(Folder folder);
    Task<bool> DeleteAsync(int id);
}
