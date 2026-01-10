

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

        public async Task<List<Folder>> GetDomainData(Guid domainId, string userId)
        {
            return await _context.Folders
                .Where(f =>
                    f.UserId == userId &&
                    f.ParentFolderId == null &&
                    f.DomainId == domainId)
                .OrderBy(f => f.FolderPosition)
                .ToListAsync();
        }

        public async Task<bool> ApplyDomain(Guid folderId, Guid domainId, string userId)
        {
            var folder = await _context.Folders
                .FirstOrDefaultAsync(f =>
                    f.Id == folderId &&
                    f.UserId == userId &&
                    f.ParentFolderId == null);

            if (folder == null) return false;

            folder.DomainId = domainId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
