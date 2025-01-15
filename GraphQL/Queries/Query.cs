namespace Trace.GraphQL.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Logic.File;
    using Trace.Service.Logic.Folder;

    public class Query
    {
        [Authorize]
        [GraphQLName("getFolders")]
        public async Task<IEnumerable<Folder>> GetFolders(
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetAllFoldersAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFolderById")]
        public async Task<Folder> GetFolderById(
            int id,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetFolderByIdAsync(id, userId);
        }

        [Authorize]
        [GraphQLName("getFiles")]
        public async Task<IEnumerable<File>> GetFiles(
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await fileService.GetAllFilesAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFileById")]
        public async Task<File> GetFileById(
            Guid id,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await fileService.GetFileByIdAsync(id, userId);
        }
    }
}
