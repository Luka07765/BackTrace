namespace Trace.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;
using Trace.Repository;
using Trace.GraphQL.Inputs;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;

    public FolderService(IFolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
    {
        return await _folderRepository.GetAllAsync();
    }

    public async Task<Folder> GetFolderByIdAsync(int id)
    {
        return await _folderRepository.GetByIdAsync(id);
    }

    public async Task<Folder> CreateFolderAsync(FolderInput input)
    {
        var folder = new Folder
        {
            Title = input.Title,
            ParentFolderId = input.ParentFolderId
        };

        return await _folderRepository.CreateAsync(folder);
    }

    public async Task<Folder> UpdateFolderAsync(int id, FolderInput input)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
            throw new Exception("Folder not found");

        folder.Title = input.Title;
        folder.ParentFolderId = input.ParentFolderId;

        return await _folderRepository.UpdateAsync(folder);
    }

    public async Task<bool> DeleteFolderAsync(int id)
    {
        return await _folderRepository.DeleteAsync(id);
    }
}
