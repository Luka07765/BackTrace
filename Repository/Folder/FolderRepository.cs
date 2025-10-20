using Microsoft.EntityFrameworkCore;

namespace Trace.Repository.Folder;
using System.Threading.Tasks;
using Trace.Data;
using Trace.Models.Logic;

public class FolderRepository : IFolderRepository
{
    private readonly ApplicationDbContext _context;

    public FolderRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    


    //MUTATIONS

    public async Task<Folder> UpdateFolderAsync(Folder folder)
    {
        _context.Folders.Update(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    //public async Task<bool> DeleteFolderAsync(Guid id, string userId)
    //{
    //    var folder = await GetFolderByIdAsync(id, userId);
    //    if (folder == null) return false;

    //    _context.Folders.Remove(folder);
    //    await _context.SaveChangesAsync();
    //    return true;
    //}
}
