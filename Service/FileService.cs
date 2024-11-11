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

    public async Task<IEnumerable<File>> GetAllFilesAsync()
    {
        return await _fileRepository.GetAllAsync();
    }

    public async Task<File> GetFileByIdAsync(int id)
    {
        return await _fileRepository.GetByIdAsync(id);
    }

    public async Task<File> CreateFileAsync(FileInput input)
    {
        var file = new File
        {
            Title = input.Title,
            Content = input.Content,
            FolderId = input.FolderId
        };

        return await _fileRepository.CreateAsync(file);
    }

    public async Task<File> UpdateFileAsync(int id, FileInput input)
    {
        var file = await _fileRepository.GetByIdAsync(id);
        if (file == null)
            throw new Exception("File not found");

        file.Title = input.Title;
        file.Content = input.Content;
        file.FolderId = input.FolderId;

        return await _fileRepository.UpdateAsync(file);
    }

    public async Task<bool> DeleteFileAsync(int id)
    {
        return await _fileRepository.DeleteAsync(id);
    }
}
