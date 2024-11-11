namespace Trace.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;


public interface IFileRepository
{
    Task<IEnumerable<File>> GetAllAsync();
    Task<File> GetByIdAsync(int id);
    Task<File> CreateAsync(File file);
    Task<File> UpdateAsync(File file);
    Task<bool> DeleteAsync(int id);
}
