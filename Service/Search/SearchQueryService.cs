namespace Trace.Service.Search
{
    using Microsoft.EntityFrameworkCore;
    using Trace.Data;
    using Trace.DTO;

    public class SearchQueryService : ISearchQueryService
    {
        private const int MaxResults = 25;
        private const int CandidateLimit = 100;   // per type, before final scoring
        private const int MaxTermLength = 100;
        private const int FolderBonus = 15;       // folders win ties against files
        private const int DepthPenalty = 4;       // shallower items rank a bit higher
        private const string LikeEscape = "\\";
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

        private readonly ApplicationDbContext _context;

        public SearchQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(string userId, string term)
        {
            term = Normalize(term);
            if (term.Length == 0)
                return Array.Empty<SearchResultDto>();

            var tokens = term.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lower = term.ToLowerInvariant();
            var escaped = EscapeLike(term);
            var prefix = $"{escaped}%";
            var wordPrefix = $"% {escaped}%";
            var contains = $"%{escaped}%";
            var tokenPatterns = tokens.Select(t => $"%{EscapeLike(t)}%").ToArray();

            // 1. Candidates: every word must appear in the title
            var folderQuery = _context.Folders.AsNoTracking().Where(f => f.UserId == userId);
            var fileQuery = _context.Files.AsNoTracking().Where(f => f.UserId == userId);

            foreach (var pattern in tokenPatterns)
            {
                folderQuery = folderQuery.Where(f => EF.Functions.ILike(f.Title, pattern, LikeEscape));
                fileQuery = fileQuery.Where(f => EF.Functions.ILike(f.Title, pattern, LikeEscape));
            }

            // Best matches first in the database, so the limit never cuts them off
            var folders = await folderQuery
                .OrderBy(f =>
                    f.Title.ToLower() == lower ? 0 :
                    EF.Functions.ILike(f.Title, prefix, LikeEscape) ? 1 :
                    EF.Functions.ILike(f.Title, wordPrefix, LikeEscape) ? 2 :
                    EF.Functions.ILike(f.Title, contains, LikeEscape) ? 3 : 4)
                .ThenBy(f => f.Title.Length)
                .ThenBy(f => f.Title)
                .Select(f => new { f.Id, f.Title, f.ParentFolderId })
                .Take(CandidateLimit)
                .ToListAsync();

            var files = await fileQuery
                .OrderBy(f =>
                    f.Title.ToLower() == lower ? 0 :
                    EF.Functions.ILike(f.Title, prefix, LikeEscape) ? 1 :
                    EF.Functions.ILike(f.Title, wordPrefix, LikeEscape) ? 2 :
                    EF.Functions.ILike(f.Title, contains, LikeEscape) ? 3 : 4)
                .ThenBy(f => f.Title.Length)
                .ThenBy(f => f.Title)
                .Select(f => new { f.Id, f.Title, f.FolderId })
                .Take(CandidateLimit)
                .ToListAsync();

            if (folders.Count == 0 && files.Count == 0)
                return Array.Empty<SearchResultDto>();

            // 2. Folder tree, only needed when there is something to show
            var folderMap = await _context.Folders
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => new FolderNode(f.Id, f.Title, f.ParentFolderId))
                .ToDictionaryAsync(f => f.Id);

            // 3. Score and pick the best overall
            var scored = new List<(SearchResultDto Dto, int Score)>(folders.Count + files.Count);

            foreach (var folder in folders)
            {
                var match = Match(folder.Title, term, tokens);
                if (match is null) continue;

                var crumbs = BuildBreadcrumbs(folder.ParentFolderId, folderMap); // parents only
                scored.Add((new SearchResultDto
                {
                    Id = folder.Id,
                    Title = folder.Title,
                    Type = "Folder",
                    Breadcrumbs = crumbs,
                    MatchRank = match.Rank,
                    MatchStart = match.Start,
                    MatchLength = match.Length,
                }, match.Score + FolderBonus - crumbs.Count * DepthPenalty));
            }

            foreach (var file in files)
            {
                var match = Match(file.Title, term, tokens);
                if (match is null) continue;

                var crumbs = BuildBreadcrumbs(file.FolderId, folderMap);
                scored.Add((new SearchResultDto
                {
                    Id = file.Id,
                    Title = file.Title,
                    Type = "File",
                    Breadcrumbs = crumbs,
                    MatchRank = match.Rank,
                    MatchStart = match.Start,
                    MatchLength = match.Length,
                }, match.Score - crumbs.Count * DepthPenalty));
            }

            var results = scored
                .OrderByDescending(s => s.Score)
                .ThenBy(s => s.Dto.Title.Length)
                .ThenBy(s => s.Dto.Title, StringComparer.OrdinalIgnoreCase)
                .Take(MaxResults)
                .Select(s => s.Dto)
                .ToList();

            for (var i = 0; i < results.Count; i++)
                results[i].TraversalOrder = i;

            return results;
        }

        /* ---------- Matching ---------- */

        private sealed record FolderNode(Guid Id, string Title, Guid? ParentFolderId);

        // Rank: 0 exact, 1 prefix, 2 word start, 3 anywhere, 4 all words in any order
        private sealed record MatchInfo(int Score, int Rank, int Start, int Length);

        private static MatchInfo? Match(string? title, string term, string[] tokens)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            // shorter titles are a tighter match
            var lengthBonus = Math.Max(0, 40 - (title.Length - term.Length));

            if (title.Equals(term, IgnoreCase))
                return new MatchInfo(1000, 0, 0, term.Length);

            if (title.StartsWith(term, IgnoreCase))
                return new MatchInfo(850 + lengthBonus, 1, 0, term.Length);

            var wordStart = IndexOfWordStart(title, term);
            if (wordStart >= 0)
                return new MatchInfo(700 + lengthBonus, 2, wordStart, term.Length);

            var index = title.IndexOf(term, IgnoreCase);
            if (index >= 0)
                return new MatchInfo(Math.Max(300, 500 - index * 3) + lengthBonus, 3, index, term.Length);

            // "plan launch" -> "Q1 Launch plan"
            if (tokens.Length > 1 && tokens.All(t => title.Contains(t, IgnoreCase)))
            {
                var first = tokens
                    .Select(t => (Token: t, Index: title.IndexOf(t, IgnoreCase)))
                    .OrderBy(x => x.Index)
                    .First();
                var wordStarts = tokens.Count(t => IndexOfWordStart(title, t) >= 0);
                return new MatchInfo(250 + wordStarts * 20 + lengthBonus / 2, 4, first.Index, first.Token.Length);
            }

            return null;
        }

        private static int IndexOfWordStart(string title, string term)
        {
            for (var i = title.IndexOf(term, IgnoreCase); i >= 0; i = title.IndexOf(term, i + 1, IgnoreCase))
            {
                if (i == 0 || IsWordBreak(title[i - 1]))
                    return i;
            }
            return -1;
        }

        private static bool IsWordBreak(char c) =>
            char.IsWhiteSpace(c) || c is '-' or '_' or '.' or '/' or '(' or '[';

        /* ---------- Helpers ---------- */

        private static string Normalize(string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return string.Empty;

            var collapsed = string.Join(' ', term.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            return collapsed.Length > MaxTermLength ? collapsed[..MaxTermLength] : collapsed;
        }

        // Treat % and _ in the search as literal characters, not wildcards
        private static string EscapeLike(string value) =>
            value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

        private static List<BreadcrumbItemDto> BuildBreadcrumbs(Guid? startFolderId, Dictionary<Guid, FolderNode> folderMap)
        {
            var list = new List<BreadcrumbItemDto>();
            var seen = new HashSet<Guid>();
            var currentId = startFolderId;

            while (currentId is Guid id && seen.Add(id) && folderMap.TryGetValue(id, out var folder))
            {
                list.Add(new BreadcrumbItemDto { Id = folder.Id, Title = folder.Title });
                currentId = folder.ParentFolderId;
            }

            list.Reverse();
            return list;
        }
    }
}