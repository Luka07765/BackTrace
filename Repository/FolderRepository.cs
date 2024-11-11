namespace Trace.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trace.Models;
using Trace.Data;

public class FolderRepository : IFolderRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FolderRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Folder>> GetAllAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Folders
            .Include(f => f.SubFolders)
            .Include(f => f.Files)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<Folder> GetByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Folders
            .Include(f => f.SubFolders)
            .Include(f => f.Files)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Folder> CreateAsync(Folder folder)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Folders.Add(folder);
        await context.SaveChangesAsync();
        return folder;
    }

    public async Task<Folder> UpdateAsync(Folder folder)
    {
        using var context = _contextFactory.CreateDbContext();
        context.Folders.Update(folder);
        await context.SaveChangesAsync();
        return folder;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var folder = await context.Folders
            .Include(f => f.SubFolders)
            .Include(f => f.Files)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (folder == null) return false;

        context.Folders.Remove(folder);
        await context.SaveChangesAsync();
        return true;
    }
}
