namespace Trace.GraphQL.Mutations.Folders
{
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using Trace.Models.Logic;

    using Trace.Service.Folder.Modify;


    [ExtendObjectType(Name = "Mutation")]
    public class FoldersMutation
    {
        [Authorize]
        [GraphQLName("createFolder")]
        public async Task<Folder> CreateFolder(
            FolderInput input,
            [Service] IFolderModifyService folderModifyService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderModifyService.CreateFolderAsync(input, userId);
        }
    }
}
