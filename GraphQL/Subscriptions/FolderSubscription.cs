
namespace Trace.GraphQL.Subscriptions
{
    using Trace.DTO;
    using HotChocolate.Subscriptions;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using Trace.Service.Folder.Fetch.Progressive;

    public class FolderSubscription
    {
        private readonly IFolderProgressiveService _folderProgressiveService;

        public FolderSubscription(IFolderProgressiveService folderProgressiveService)
        {
            _folderProgressiveService = folderProgressiveService;
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

            await foreach (var layer in _folderProgressiveService.StreamFolderHierarchyAsync(folderId, userId, cancellationToken))
            {
                yield return layer;
            }
        }
    }

}
