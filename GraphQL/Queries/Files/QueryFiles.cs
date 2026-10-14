namespace Trace.GraphQL.Queries.Files
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
    using Trace.Service.Files.Fetch;

    [ExtendObjectType("Query")]
    public class QueryFiles
    {
        [Authorize]
        [GraphQLName("getFiles")]
        public Task<IEnumerable<File>> GetFiles(
            [Service] IFileQueryService fileQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => fileQueryService.GetAllFilesAsync(user.GetRequiredUserId(), cancellationToken);

        [Authorize]
        [GraphQLName("getFileById")]
        public Task<File?> GetFileById(
            Guid id,
            [Service] IFileQueryService fileQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => fileQueryService.GetFileByIdAsync(id, user.GetRequiredUserId(), ct: cancellationToken);

        [Authorize]
        [GraphQLName("trashedFiles")]
        public Task<List<File>> GetTrashedFiles(
            [Service] IFileQueryService fileQueryService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
            => fileQueryService.GetFileTrashedAsync(user.GetRequiredUserId(), cancellationToken);
    }
}