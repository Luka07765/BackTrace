namespace Trace.GraphQL.Queries.Profile
{
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Identity;
    using Trace.Models.Account;

    [ExtendObjectType("Query")]
    public class QueryProfile
    {
        [Authorize]
        [GraphQLName("getProfile")]
        public async Task<User> GetProfile(
            ClaimsPrincipal user,
            [Service] UserManager<User> userManager)
        {
            var userId = user.FindFirstValue("CustomUserId");

            if (string.IsNullOrEmpty(userId))
            {
                throw new GraphQLException(
                    new Error("User ID not found in claims", "UNAUTHORIZED"));
            }

            var appUser = await userManager.FindByIdAsync(userId);

            if (appUser == null)
            {
                throw new GraphQLException(
                    new Error("User not found", "NOT_FOUND"));
            }

            return appUser;
        }
    }
}
