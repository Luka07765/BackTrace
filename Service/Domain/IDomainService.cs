namespace Trace.Service.Domain
{
    using System.Threading.Tasks;
    using Trace.Models.Logic;
    public interface IDomainService
    {
        Task<List<Domain>> GetDomains(string userId);
        Task<Domain> CreateAsync(string userId, string title);
        Task<Domain> UpdateAsync(Guid id, string title, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
    }

}
