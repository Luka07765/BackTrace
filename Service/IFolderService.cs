﻿namespace Trace.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models;

    public interface IFolderService
    {
        Task<IEnumerable<Folder>> GetAllFoldersAsync(string userId);
        Task<Folder> GetFolderByIdAsync(int id, string userId);
        Task<Folder> CreateFolderAsync(FolderInput input, string userId);
        Task<Folder> UpdateFolderAsync(int id, FolderInput input, string userId);
        Task<bool> DeleteFolderAsync(int id, string userId);
    }
}
