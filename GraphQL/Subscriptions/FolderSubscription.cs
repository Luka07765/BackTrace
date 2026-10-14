namespace Trace.GraphQL.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using HotChocolate;
    using HotChocolate.Types;
    using Trace.DTO;
    using Trace.Service.Folder.Fetch.Progressive;

    public class FolderSubscription
    {
        [GraphQLName("getProgressive")]
        [SubscribeAndResolve]
        public IAsyncEnumerable<FolderLayerPayload> FolderProgressiveAsync(
            Guid folderId,
            ClaimsPrincipal user,
            [Service] IFolderProgressiveService folderProgressiveService,
            CancellationToken cancellationToken)
        {
            var userId = user.GetRequiredUserId();

            return folderProgressiveService.StreamFolderHierarchyAsync(folderId, userId, cancellationToken);
        }
    }
}
