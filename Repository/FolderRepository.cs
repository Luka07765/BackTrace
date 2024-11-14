﻿using Microsoft.EntityFrameworkCore;

namespace Trace.Repository;
using System.Threading.Tasks;
using Trace.Data;
using Trace.Models;




public class FolderRepository : IFolderRepository
{
    private readonly ApplicationDbContext _context;

    public FolderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId)
    {
        return await _context.Folders
            .Where(f => f.UserId == userId)
            .Include(f => f.SubFolders)
            .Include(f => f.Files)
            .ToListAsync();
    }

    public async Task<Folder> GetFolderByIdAsync(int id, string userId)
    {
        return await _context.Folders
            .Include(f => f.SubFolders)
            .Include(f => f.Files)
            .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
    }

    public async Task<Folder> CreateFolderAsync(Folder folder)
    {
        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task<Folder> UpdateFolderAsync(Folder folder)
    {
        _context.Folders.Update(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task<bool> DeleteFolderAsync(int id, string userId)
    {
        var folder = await GetFolderByIdAsync(id, userId);
        if (folder == null) return false;

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();
        return true;
    }
}