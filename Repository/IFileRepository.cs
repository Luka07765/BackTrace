﻿namespace Trace.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trace.Models;


public interface IFileRepository
{
    Task<IEnumerable<File>> GetAllFilesAsync(string userId);
    Task<File> GetFileByIdAsync(int id, string userId);
    Task<File> CreateFileAsync(File file);
    Task<File> UpdateFileAsync(File file);
    Task<bool> DeleteFileAsync(int id, string userId);
}
