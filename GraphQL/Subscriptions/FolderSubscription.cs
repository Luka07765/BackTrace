
namespace Trace.GraphQL.Subscriptions
{
    using Trace.DTO;
    using HotChocolate.Subscriptions;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using Trace.Service.Logic.Folder;

    public class FolderSubscription
    {
        private readonly IFolderService _folderService;

        public FolderSubscription(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [SubscribeAndResolve] // ✅ use this instead of [Subscribe]
        public async IAsyncEnumerable<FolderLayerPayload> FolderProgressiveAsync(
            Guid folderId,
            ClaimsPrincipal user,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                yield break;

            await foreach (var layer in _folderService.StreamFolderHierarchyAsync(folderId, userId, cancellationToken))
            {
                yield return layer;
            }
        }
    }

}
