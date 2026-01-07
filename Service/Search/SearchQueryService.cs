namespace Trace.Service.Search
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.DTO;
    using Trace.Models.Logic;

    public class SearchQueryService : ISearchQueryService
    {
        private const int MaxResults = 25;
        private readonly ApplicationDbContext _context;

        public SearchQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static int GetMatchRank(string title, string term)
        {
            if (title.Equals(term, StringComparison.OrdinalIgnoreCase))
                return 0; // exact

            if (title.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                return 1; // prefix

            return 2;     // substring
        }
        private static (int start, int length)? GetMatchPosition(string title, string term)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(term))
                return null;

            var index = title
                .IndexOf(term, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
                return null;

            return (index, term.Length);
        }

        private static List<BreadcrumbItemDto> BuildBreadcrumbs(
            Guid startFolderId,
            Dictionary<Guid, Folder> folderMap)
        {
            var list = new List<BreadcrumbItemDto>();
            Guid? currentId = startFolderId;

            while (currentId != null &&
                   folderMap.TryGetValue(currentId.Value, out var folder))
            {
                list.Add(new BreadcrumbItemDto
                {
                    Id = folder.Id,
                    Title = folder.Title
                });

                currentId = folder.ParentFolderId;
            }

            list.Reverse();
            return list;
        }

        public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
            string userId,
            string term)
        {
            term = (term ?? string.Empty).Trim();
            if (term.Length == 0)
                return Array.Empty<SearchResultDto>();

            // 1️⃣ Load folder structure (required for breadcrumbs)
            var folderMap = await _context.Folders
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => new Folder
                {
                    Id = f.Id,
                    Title = f.Title,
                    ParentFolderId = f.ParentFolderId
                })
                .ToDictionaryAsync(f => f.Id);

            // 2️⃣ DB pre-filter — ORDERED + LIMITED (FIX)
            var matchedFolders = await _context.Folders
                .AsNoTracking()
                .Where(f =>
                    f.UserId == userId &&
                    EF.Functions.ILike(f.Title, $"%{term}%"))
                .OrderBy(f => f.Title)            // ✅ REQUIRED
                .Select(f => new { f.Id, f.Title })
                .Take(MaxResults)
                .ToListAsync();

            var matchedFiles = await _context.Files
                .AsNoTracking()
                .Where(f =>
                    f.UserId == userId &&
                    EF.Functions.ILike(f.Title, $"%{term}%"))
                .OrderBy(f => f.Title)            // ✅ REQUIRED
                .Select(f => new { f.Id, f.Title, f.FolderId })
                .Take(MaxResults)
                .ToListAsync();

            var results = new List<SearchResultDto>(matchedFolders.Count + matchedFiles.Count);
            int traversalIndex = 0;

            // 3️⃣ Folder hits
            foreach (var folder in matchedFolders)
            {
                var match = GetMatchPosition(folder.Title, term);
                results.Add(new SearchResultDto
                {
                    Id = folder.Id,
                    Title = folder.Title,
                    Type = "Folder",
                    Breadcrumbs = BuildBreadcrumbs(folder.Id, folderMap),
                    TraversalOrder = traversalIndex++,
                    MatchRank = GetMatchRank(folder.Title, term),
                    MatchStart = match?.start,
                    MatchLength = match?.length

                });
            }

            // 4️⃣ File hits
            foreach (var file in matchedFiles)
            {
                var match = GetMatchPosition(file.Title, term);
                results.Add(new SearchResultDto
                {
                    Id = file.Id,
                    Title = file.Title,
                    Type = "File",
                    Breadcrumbs = BuildBreadcrumbs(file.FolderId, folderMap),
                    TraversalOrder = traversalIndex++,
                    MatchRank = GetMatchRank(file.Title, term),
                    MatchStart = match?.start,
                    MatchLength = match?.length
                });
            }

            
            return results
                 .OrderBy(r => r.MatchRank)
                .ThenBy(r => r.Breadcrumbs.Count)
                .ThenBy(r => r.TraversalOrder)
                .ThenBy(r => r.Type == "Folder" ? 0 : 1)
                .ThenBy(r => r.Title)
                .ToList();
        }
    }
}
