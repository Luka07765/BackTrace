
namespace Trace.GraphQL.Queries.Tag
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Tag;
    using Trace.Models.TagSystem;
    using Trace.Service.Folder;

    [ExtendObjectType("Query")]
    public class QueryTags
    {

        [Authorize]
        [GraphQLName("getTags")]
        public async Task<IEnumerable<Tag>> GetTags(
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await tagService.GetAllTagsAsync(userId);
        }

        [Authorize]
        [GraphQLName("getTagById")]
        public async Task<Tag> GetTagById(
            Guid id,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await tagService.GetTagByIdAsync(id, userId);
        }

        [Authorize]
        [GraphQLName("getFilesByTag")]
        public async Task<IEnumerable<File>> GetFilesByTag(
            Guid tagId,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await tagService.GetFilesByTagAsync(tagId, userId);
        }
    }
}
