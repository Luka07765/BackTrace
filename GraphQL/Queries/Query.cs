namespace Trace.GraphQL.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trace.Models;
    using Trace.Service;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;

    public class Query
    {
        [Authorize]
        [GraphQLName("getFolders")]
        public async Task<IEnumerable<Folder>> GetFolders(
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await folderService.GetAllFoldersAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFolderById")]
        public async Task<Folder> GetFolderById(
            int id,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await folderService.GetFolderByIdAsync(id, userId);
        }

        [Authorize]
        [GraphQLName("getFiles")]
        public async Task<IEnumerable<File>> GetFiles(
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await fileService.GetAllFilesAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFileById")]
        public async Task<File> GetFileById(
            int id,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await fileService.GetFileByIdAsync(id, userId);
        }
    }
}