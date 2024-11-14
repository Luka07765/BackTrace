namespace Trace.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Repository;
using Trace.Models;
using Trace.GraphQL.Inputs;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<IEnumerable<File>> GetAllFilesAsync(string userId)
    {
        return await _fileRepository.GetAllFilesAsync(userId);
    }

    public async Task<File> GetFileByIdAsync(int id, string userId)
    {
        return await _fileRepository.GetFileByIdAsync(id, userId);
    }

    public async Task<File> CreateFileAsync(FileInput input, string userId)
    {
        var file = new File
        {
            Title = input.Title,
            Content = input.Content,
            FolderId = input.FolderId,
            UserId = userId
        };
        return await _fileRepository.CreateFileAsync(file);
    }

    public async Task<File> UpdateFileAsync(int id, FileInput input, string userId)
    {
        var file = await _fileRepository.GetFileByIdAsync(id, userId);
        if (file == null)
        {
            throw new UnauthorizedAccessException("You do not have permission to edit this file.");
        }

        file.Title = input.Title;
        file.Content = input.Content;
        file.FolderId = input.FolderId;
        return await _fileRepository.UpdateFileAsync(file);
    }

    public async Task<bool> DeleteFileAsync(int id, string userId)
    {
        var file = await _fileRepository.GetFileByIdAsync(id, userId);
        if (file == null)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this file.");
        }

        return await _fileRepository.DeleteFileAsync(id, userId);
    }
}