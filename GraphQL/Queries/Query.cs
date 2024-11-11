namespace Trace.GraphQL.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.Models;
    using Trace.Service;
    using HotChocolate;

    public class Query
    {
        [GraphQLName("getFolders")]
        public async Task<IEnumerable<Folder>> GetFolders([Service] IFolderService folderService)
        {
            return await folderService.GetAllFoldersAsync();
        }

        [GraphQLName("getFolderById")]
        public async Task<Folder> GetFolderById(int id, [Service] IFolderService folderService)
        {
            return await folderService.GetFolderByIdAsync(id);
        }

        [GraphQLName("getFiles")]
        public async Task<IEnumerable<File>> GetFiles([Service] IFileService fileService)
        {
            return await fileService.GetAllFilesAsync();
        }

        [GraphQLName("getFileById")]
        public async Task<File> GetFileById(int id, [Service] IFileService fileService)
        {
            return await fileService.GetFileByIdAsync(id);
        }
    }
}
