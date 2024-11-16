namespace Trace.GraphQL.Mutations
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Logic.File;
    using Trace.Service.Logic.Folder;

    public class Mutation
    {
        [Authorize]
        [GraphQLName("createFolder")]
        public async Task<Folder> CreateFolder(
            FolderInput input,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await folderService.CreateFolderAsync(input, userId);
        }

        [Authorize]
        [GraphQLName("updateFolder")]
        public async Task<Folder> UpdateFolder(
            int id,
            FolderInput input,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                return await folderService.UpdateFolderAsync(id, input, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UNAUTHORIZED"));
            }
        } 

        [Authorize]
        [GraphQLName("deleteFolder")]
        public async Task<bool> DeleteFolder(
            int id,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                return await folderService.DeleteFolderAsync(id, userId);

            }
            catch (UnauthorizedAccessException ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UNAUTHORIZED"));
            }

        }

        [Authorize]
        [GraphQLName("createFile")]
        public async Task<File> CreateFile(
            FileInput input,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await fileService.CreateFileAsync(input, userId);
        }

        [Authorize]
        [GraphQLName("updateFile")]
        public async Task<File> UpdateFile(
            int id,
            FileInput input,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                return await fileService.UpdateFileAsync(id, input, userId);

            }
            catch (UnauthorizedAccessException ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UNAUTHORIZED"));
            }

        }

        [Authorize]
        [GraphQLName("deleteFile")]
        public async Task<bool> DeleteFile(
            int id,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                return await fileService.DeleteFileAsync(id, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UNAUTHORIZED"));
            }

        }
    }
}