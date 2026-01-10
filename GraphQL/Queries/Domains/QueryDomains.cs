namespace Trace.GraphQL.Queries.Domains
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Domain;

    [ExtendObjectType("Query")]
    public class QueryDomains
    {
        [Authorize]
        [GraphQLName("getDomains")]
        public async Task<IEnumerable<Domain>> GetDomains(
            [Service] IDomainService domainQueryService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");

            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(
                    new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            return await domainQueryService.GetDomains(userId);
        }
    }
}
