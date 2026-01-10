namespace Trace.GraphQL.Queries.Files
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Files.Fetch;

    [ExtendObjectType("Query")]
    public class QueryFiles
    {
        [Authorize]
        [GraphQLName("getFiles")]
        public async Task<IEnumerable<File>> GetFiles(
            [Service] IFileQueryService fileQueryService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await fileQueryService.GetAllFilesAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFileById")]
        public async Task<File> GetFileById(
            Guid id,
            [Service] IFileQueryService fileService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await fileService.GetFileByIdAsync(id, userId);
        }



        [Authorize]
        [GraphQLName("trashedFiles")]
        public async Task<List<File>> GetTrashedFiles(
         [Service] IFileQueryService fileQueryService,
         ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");

            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("Unauthorized", "UNAUTHORIZED"));

            return await fileQueryService.GetFileTrashedAsync(userId);
        }
    }
}
