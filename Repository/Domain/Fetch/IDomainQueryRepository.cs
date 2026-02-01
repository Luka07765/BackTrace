namespace Trace.Repository.Domain.Fetch
{
    using Trace.Models.Data;

    public interface IDomainQueryRepository
    {
        Task<List<Domain>> GetDomains(string userId);
    }
}
