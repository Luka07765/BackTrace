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
    }
}
