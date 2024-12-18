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
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

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
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
                }

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
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
                }

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
         CreateFileInput input,
        [Service] IFileService fileService,
        [Service] IFolderService folderService,
     ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            var isOwner = await folderService.IsFolderOwnedByUserAsync(input.FolderId, userId);
            if (!isOwner)
            {
                throw new GraphQLException(new Error("You do not have permission to add files to this folder", "FORBIDDEN"));
            }

            return await fileService.CreateFileAsync(input, userId);
        }




        [Authorize]
        [GraphQLName("updateFile")]
        public async Task<File> UpdateFile(
            int id,
            UpdateFileInput input,
            [Service] IFileService fileService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
                }

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
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
                }

                return await fileService.DeleteFileAsync(id, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UNAUTHORIZED"));
            }
        }
    }
}
