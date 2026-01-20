namespace Trace.Repository.Domain.Fetch
{
    using Trace.Models.Logic;
    public interface IDomainQueryRepository
    {
        Task<List<Domain>> GetDomains(string userId);
    }
}
