
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
