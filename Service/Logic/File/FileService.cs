namespace Trace.Service.Logic.File;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.GraphQL.Inputs;
using Trace.Models.Logic;
using Trace.Repository.Files;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }





    public async Task<File> UpdateFileAsync(Guid id, UpdateFileInput input, string userId)
    {
        // Ensure the file exists and belongs to the user
        //var file = await _fileRepository.GetFileByIdAsync(id, userId);
        //if (file == null)
        //{
        //    throw new UnauthorizedAccessException("You do not have permission to edit this file.");
        //}

        // Directly call SaveFileDeltaAsync for the delta update
        return await _fileRepository.SaveFileDeltaAsync(id, input.FolderId, input.Colors, input.Title, input.Content, userId, input.FilePosition, input.IconId);
    }



}