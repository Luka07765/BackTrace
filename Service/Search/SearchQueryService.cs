namespace Trace.Service.Search
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.DTO;

    public class SearchQueryService : ISearchQueryService
    {
        private readonly ApplicationDbContext _context;

        public SearchQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
            string userId,
            string term)
        {
            term = (term ?? "").Trim();
            if (term.Length == 0)
                return Array.Empty<SearchResultDto>();

            var folders = _context.Folders
                .AsNoTracking()
                .Where(f =>
                    f.UserId == userId &&
                    EF.Functions.ILike(f.Title, $"%{term}%"))
                .Select(f => new SearchResultDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Type = "Folder"
                });

            var files = _context.Files
                .AsNoTracking()
                .Where(f =>
                    f.UserId == userId &&
                    EF.Functions.ILike(f.Title, $"%{term}%"))
                .Select(f => new SearchResultDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Type = "File"
                });

            return await folders
                .Union(files)
                .OrderBy(r => r.Title)
                .Take(50)
                .ToListAsync();
        }
    }

}
