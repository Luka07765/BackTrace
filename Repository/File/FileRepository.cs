namespace Trace.Repository.File;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trace.Data;
using Trace.Models.Logic;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;

    public FileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
    {
        return await _context.Files
            .Where(f => f.UserId == userId)
            .Include(f => f.Folder)
            .ToListAsync();
    }

    public async Task<File> GetFileByIdAsync(int id, string userId)
    {
        return await _context.Files
            .Include(f => f.Folder)
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
    }

    public async Task<File> CreateFileAsync(File file)
    {
        _context.Files.Add(file);
        await _context.SaveChangesAsync();
        return file;
    }

    public async Task<File> UpdateFileAsync(File file)
    {
        _context.Files.Update(file);
        await _context.SaveChangesAsync();
        return file;
    }

    public async Task<bool> DeleteFileAsync(int id, string userId)
    {
        var file = await GetFileByIdAsync(id, userId);
        if (file == null) return false;

        _context.Files.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }
}

