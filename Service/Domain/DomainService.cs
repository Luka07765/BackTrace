
namespace Trace.Service.Domain
{
    using Trace.Repository.Domain;

    using System.Threading.Tasks;
    using Trace.Models.Logic;

    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _repository;

        public DomainService(IDomainRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Domain>> GetDomains(string userId)
            => _repository.GetDomains(userId);

        public async Task<Domain> CreateAsync(string userId, string title)
        {
            var domain = new Domain
            {
                Id = Guid.NewGuid(),
                Title = title,
                UserId = userId
            };

            return await _repository.CreateAsync(domain);
        }

        public async Task<Domain> UpdateAsync(Guid id, string title, string userId)
        {
            var domains = await _repository.GetDomains(userId);
            var domain = domains.FirstOrDefault(d => d.Id == id);

            if (domain == null) return null;

            domain.Title = title;
            return await _repository.UpdateAsync(domain);
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            var domains = await _repository.GetDomains(userId);
            var domain = domains.FirstOrDefault(d => d.Id == id);

            if (domain == null) return false;

            return await _repository.DeleteAsync(domain);
        }
    }

}
