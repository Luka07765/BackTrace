namespace Trace.GraphQL.Mutations.Domains
{
    using System;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Authorization;
    using System.Security.Claims;
    using Trace.Models.Logic;
    using Trace.Service.Domain;

    [ExtendObjectType(Name = "Mutation")]
    public class DomainsMutation
    {
        [Authorize]
        [GraphQLName("createDomain")]
        public async Task<Domain> CreateDomain(
            string title,
            [Service] IDomainService domainModifyService,
            ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

            return await domainModifyService.CreateAsync(userId,title);
        }

        [Authorize]
        [GraphQLName("updateDomain")]
        public async Task<Domain?> UpdateDomain(
            Guid id,
            string title,
            [Service] IDomainService domainModifyService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

                var updated = await domainModifyService.UpdateAsync(id, title, userId);

                if (updated == null)
                    throw new GraphQLException(new Error("Domain not found", "NOT_FOUND"));

                return updated;
            }
            catch (Exception ex)
            {
                throw new GraphQLException(new Error(ex.Message, "UPDATE_FAILED"));
            }
        }

        [Authorize]
        [GraphQLName("deleteDomain")]
        public async Task<bool> DeleteDomain(
            Guid id,
            [Service] IDomainService domainModifyService,
            ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue("CustomUserId");
                if (string.IsNullOrEmpty(userId))
                    throw new GraphQLException(new Error("User ID not found in claims", "UNAUTHORIZED"));

                var deleted = await domainModifyService.DeleteAsync(id, userId);

                if (!deleted)
                    throw new GraphQLException(new Error("Domain not found", "NOT_FOUND"));

                return true;
            }
            catch (Exception ex)
            {
                throw new GraphQLException(new Error(ex.Message, "DELETE_FAILED"));
            }
        }

        [Authorize]
        [GraphQLName("applyDomain")]
        public async Task<bool> ApplyDomain(
        Guid folderId,
        Guid domainId,
        [Service] IDomainService service,
        ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue("CustomUserId");
            if (string.IsNullOrEmpty(userId))
                throw new GraphQLException(new Error("UNAUTHORIZED", "UNAUTHORIZED"));

            var result = await service.ApplyDomain(folderId, domainId, userId);

            if (!result)
                throw new GraphQLException(new Error("Folder not found or not root", "NOT_FOUND"));

            return true;
        }




    }
}
