namespace Trace.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trace.Data;
using Trace.Models;

public class FileRepository : IFileRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FileRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<File>> GetAllAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Files
            .Include(f => f.Folder)
            .ToListAsync();
    }

    public async Task<File> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Files
            .Include(f => f.Folder)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<File> CreateAsync(File file)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Files.Add(file);
        await context.SaveChangesAsync();
        return file;
    }

    public async Task<File> UpdateAsync(File file)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Files.Update(file);
        await context.SaveChangesAsync();
        return file;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var file = await context.Files
            .Include(f => f.Folder)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (file == null) return false;

        context.Files.Remove(file);
        await context.SaveChangesAsync();
        return true;
    }
}
