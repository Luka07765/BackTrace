namespace Trace.GraphQL
{
    using System.Security.Claims;
    using HotChocolate;

    public static class TraceClaimTypes
    {
        // Use this both where the JWT is issued and where it is read.
        public const string UserId = "CustomUserId";
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetRequiredUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(TraceClaimTypes.UserId);
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

            return userId;
        }
    }
}