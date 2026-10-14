namespace Trace.GraphQL.Queries.Folders
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using HotChocolate.Types;
    using Trace.Models.Logic;
    using Trace.Service.Folder.Fetch.Query;

    [ExtendObjectType("Query")]
    public class QueryFolders
    {
        [Authorize]
        [GraphQLName("getFolders")]
        public Task<IEnumerable<Folder>> GetFolders(
            [Service] IFolderQueryService folderQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => folderQueryService.GetAllFoldersAsync(user.GetRequiredUserId(), cancellationToken);

        [Authorize]
        [GraphQLName("getFolderById")]
        public Task<Folder?> GetFolderById(
            Guid id,
            [Service] IFolderQueryService folderQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => folderQueryService.GetFolderByIdAsync(id, user.GetRequiredUserId(), cancellationToken);

        [Authorize]
        [GraphQLName("getRootFolders")]
        public Task<IEnumerable<Folder>> GetRootFolders(
            [Service] IFolderQueryService folderQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => folderQueryService.GetRootFoldersAsync(user.GetRequiredUserId(), cancellationToken);
    }
}