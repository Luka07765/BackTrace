namespace Trace.GraphQL.Queries.Search
{
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.DTO;
    using Trace.Service.Search;

    [ExtendObjectType("Query")]
    public class QuerySearch
    {
        [Authorize]
        [GraphQLName("search")]
        public async Task<IReadOnlyList<SearchResultDto>> Search(
            string term,
            [Service] ISearchQueryService searchQueryService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(
                    new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await searchQueryService.SearchAsync(userId, term);
        }
    }

}
