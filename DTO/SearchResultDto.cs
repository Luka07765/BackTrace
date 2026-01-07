namespace Trace.DTO
{
    public class BreadcrumbItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
    }

    public class SearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<BreadcrumbItemDto> Breadcrumbs { get; set; } = new();
        public int TraversalOrder { get; set; }
        public int MatchRank { get; set; }
        public int? MatchStart { get; set; }
        public int? MatchLength { get; set; }
    }
}
