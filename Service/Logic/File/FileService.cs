namespace Trace.Service.Logic.File;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.GraphQL.Inputs;
using Trace.Models.Logic;
using Trace.Repository.File;

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

    public async Task<File> GetFileByIdAsync(Guid id, string userId)
    {
        return await _fileRepository.GetFileByIdAsync(id, userId);
    }

    public async Task<File> CreateFileAsync(CreateFileInput input, string userId)
    {
        var file = new File
        {
            Title = input.Title,
            Content = input.Content ?? "", 
            FolderId = input.FolderId,
            UserId = userId,
            Colors = input.Colors
        };

        return await _fileRepository.CreateFileAsync(file);
    }



    public async Task<File> UpdateFileAsync(Guid id, UpdateFileInput input, string userId)
    {
        // Ensure the file exists and belongs to the user
        var file = await _fileRepository.GetFileByIdAsync(id, userId);
        if (file == null)
        {
            throw new UnauthorizedAccessException("You do not have permission to edit this file.");
        }

        // Directly call SaveFileDeltaAsync for the delta update
        return await _fileRepository.SaveFileDeltaAsync(id, input.FolderId, input.Colors, input.Title, input.Content, userId);
    }


    public async Task<bool> DeleteFileAsync(Guid id, string userId)
    {
        var file = await _fileRepository.GetFileByIdAsync(id, userId);
        if (file == null)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this file.");
        }

        return await _fileRepository.DeleteFileAsync(id, userId);
    }
}