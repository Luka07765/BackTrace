using Trace.DTO;

namespace Trace.Service.Search
{
    public interface ISearchQueryService
    {
        Task<IReadOnlyList<SearchResultDto>> SearchAsync(
        string userId,
        string term);
    }
}
