

namespace Trace.GraphQL.Mutations.Tags
{
    using System.Threading.Tasks;
    using Trace.GraphQL.Inputs;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
 
    using Trace.Models.TagSystem;
    using Trace.Service.Tag;

    [ExtendObjectType(Name = "Mutation")]
    public class TagsMutation
    {


        [Authorize]
        [GraphQLName("createTag")]
        public async Task<Tag> CreateTag(
            TagInput.CreateTagInput input,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            await tagService.CreateTagAsync(userId, input.Title, input.Color, input.IconId);

            return new Tag
            {
                Title = input.Title,
                Color = input.Color,
                IconId = input.IconId,
                UserId = userId
            };
        }

        [Authorize]
        [GraphQLName("updateTag")]
        public async Task<Tag> UpdateTag(
            TagInput.UpdateTagInput input,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            var tag = new Tag
            {
                Id = input.Id,
                Title = input.Title,
                Color = input.Color,
                IconId = input.IconId,
                UserId = userId
            };

            await tagService.UpdateTagAsync(tag);
            return tag;
        }

        [Authorize]
        [GraphQLName("deleteTag")]
        public async Task<bool> DeleteTag(
            Guid id,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            await tagService.DeleteTagAsync(id, userId);
            return true;
        }

        [Authorize]
        [GraphQLName("assignTagToFile")]
        public async Task<bool> AssignTagToFile(
            TagInput.AssignTagInput input,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            await tagService.AssignTagToFileAsync(input.FileId, input.TagId, userId);
            return true;
        }

        [Authorize]
        [GraphQLName("removeTagFromFile")]
        public async Task<bool> RemoveTagFromFile(
            TagInput.AssignTagInput input,
            [Service] ITagService tagService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            await tagService.RemoveTagFromFileAsync(input.FileId, input.TagId, userId);
            return true;
        }

    }
}
