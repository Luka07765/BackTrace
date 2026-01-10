

namespace Trace.Repository.Domain
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Logic;
    public class DomainRepository : IDomainRepository
    {
        private readonly ApplicationDbContext _context;

        public DomainRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<List<Domain>> GetDomains(string userId)
        {
            return await _context.Domains
                .Where(d => d.UserId == userId)
                .OrderBy(d => d.Title)
                .ToListAsync();
        }

        public async Task<Domain> CreateAsync(Domain domain)
        {
            _context.Domains.Add(domain);
            await _context.SaveChangesAsync();
            return domain;
        }

        public async Task<Domain> UpdateAsync(Domain domain)
        {
            _context.Domains.Update(domain);
            await _context.SaveChangesAsync();
            return domain;
        }

        public async Task<bool> DeleteAsync(Domain domain)
        {
            _context.Domains.Remove(domain);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
