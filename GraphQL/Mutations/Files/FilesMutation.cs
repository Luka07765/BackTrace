
namespace Trace.GraphQL.Mutations.Files
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Logic.Folder;
    using Trace.Service.Files.Modify;

    [ExtendObjectType(Name = "Mutation")]
    public class FilesMutation
    {
        [Authorize]
        [GraphQLName("updateFile")]
        public async Task<File?> UpdateFile(
             Guid id,
             UpdateFileInput input,
            [Service] IFileModifyService fileModifyService)
        {
            try
            {
                var updatedFile = await fileModifyService.UpdateFileAsync(id, input);
                if (updatedFile == null)
                    throw new GraphQLException(new Error("File not found", "NOT_FOUND"));

                return updatedFile;
            }
            catch (Exception ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UPDATE_FAILED"));
            }
        }



        [Authorize]
        [GraphQLName("deleteFile")]
        public async Task<bool> DeleteFile(
                Guid id,
                [Service] IFileModifyService fileModifyService)
        {
            try
            {
                var deleted = await fileModifyService.DeleteFileAsync(id);
                if (!deleted)
                {
                    throw new GraphQLException(new Error("File not found", "NOT_FOUND"));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new GraphQLException(new Error(ex.Message, "DELETE_FAILED"));
            }
        }



        [Authorize]
        [GraphQLName("createFile")]
        public async Task<File> CreateFileAsync(
            CreateFileInput input,
            [Service] IFileModifyService fileModifyService,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            // ✅ Extract User ID from JWT claims
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

    

        
            return await fileModifyService.CreateFileAsync(input, userId);
        }
    }


}
