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

        // =========================
        // Breadcrumb builder
        // =========================
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
                results.Add(new SearchResultDto
                {
                    Id = folder.Id,
                    Title = folder.Title,
                    Type = "Folder",
                    Breadcrumbs = BuildBreadcrumbs(folder.Id, folderMap),
                    TraversalOrder = traversalIndex++
                });
            }

            // 4️⃣ File hits
            foreach (var file in matchedFiles)
            {
                results.Add(new SearchResultDto
                {
                    Id = file.Id,
                    Title = file.Title,
                    Type = "File",
                    Breadcrumbs = BuildBreadcrumbs(file.FolderId, folderMap),
                    TraversalOrder = traversalIndex++
                });
            }

            // 5️⃣ Final deterministic ordering
            return results
                .OrderBy(r => r.Breadcrumbs.Count)
                .ThenBy(r => r.TraversalOrder)
                .ThenBy(r => r.Type == "Folder" ? 0 : 1)
                .ThenBy(r => r.Title)
                .ToList();
        }
    }
}
