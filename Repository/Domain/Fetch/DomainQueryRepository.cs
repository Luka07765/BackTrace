
namespace Trace.Repository.Domain.Fetch
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.Models.Logic;
    public class DomainQueryRepository : IDomainQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public DomainQueryRepository(ApplicationDbContext context)
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
    }
}
