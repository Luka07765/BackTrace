namespace Trace.GraphQL.Queries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Logic.File;
    using Trace.Service.Logic.Folder;
    using Trace.Service.Tag;
    using Trace.Models.TagSystem;
    public class Query
    {
        [Authorize]
        [GraphQLName("getFolders")]
        public async Task<IEnumerable<Folder>> GetFolders(
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetAllFoldersAsync(userId);
        }

        [Authorize]
        [GraphQLName("getFolderById")]
        public async Task<Folder> GetFolderById(
            Guid id,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetFolderByIdAsync(id, userId);
        }
        [Authorize]
        [GraphQLName("getRootFolders")]
        public async Task<IEnumerable<Folder>> GetRootFolders(
    [Service] IFolderService folderService,
    ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetRootFoldersAsync(userId);
        }

        [Authorize]
        [GraphQLName("getLayer")]
        public async Task<Folder> GetFolderFirstLayer(
            Guid folderId,
            [Service] IFolderService folderService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await folderService.GetFirstLayerAsync(folderId, userId);
        }


        [Authorize]
        [GraphQLName("getFolderTree")]
        public async Task<Folder> GetFolderTree(
    Guid folderId,
    [Service] IFolderService folderService,
    ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

            return await folderService.GetFolderTreeAsync(folderId, userId);
        }



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
