
namespace Trace.GraphQL.Queries.Folders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Folder.Fetch.Progressive;

    [ExtendObjectType("Query")]
    public class Progressive
    {
        [Authorize]
        [GraphQLName("getLayer")]
        public async Task<Folder> GetFolderFirstLayer(
            Guid folderId,
            [Service] IFolderProgressiveService folderProgressiveService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderProgressiveService.GetFirstLayerAsync(folderId, userId);
        }


        [Authorize]
        [GraphQLName("getFolderTree")]
        public async Task<Folder> GetFolderTree(
    Guid folderId,
    [Service] IFolderProgressiveService folderProgressiveService,
    ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

            return await folderProgressiveService.GetFolderTreeAsync(folderId, userId);
        }

    }
}
