namespace Trace.DTO
{
    public class SearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
