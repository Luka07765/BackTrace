namespace Trace.Repository.Domain
{
    using System.Threading.Tasks;
    using Trace.Models.Logic;


    public interface IDomainRepository
    {
        Task<List<Domain>> GetDomains(string userId);
        Task<Domain> CreateAsync(Domain domain);
        Task<Domain> UpdateAsync(Domain domain);
        Task<bool> DeleteAsync(Domain domain);
    }

}
