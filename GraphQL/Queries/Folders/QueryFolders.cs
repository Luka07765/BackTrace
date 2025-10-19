
namespace Trace.GraphQL.Queries.Folders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Folder.Fetch.Query;


    [ExtendObjectType("Query")]
    public class QueryFolders
    {
        [Authorize]
        [GraphQLName("getFolders")]
        public async Task<IEnumerable<Folder>> GetFolders(
            [Service] IFolderQueryService folderQueryService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderQueryService.GetAllFoldersAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFolderById")]
        public async Task<Folder> GetFolderById(
            Guid id,
            [Service] IFolderQueryService folderQueryService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderQueryService.GetFolderByIdAsync(id, userId);
        }
        [Authorize]
        [GraphQLName("getRootFolders")]
        public async Task<IEnumerable<Folder>> GetRootFolders(
    [Service] IFolderQueryService folderQueryService,
    ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderQueryService.GetRootFoldersAsync(userId);
        }
    }
}
