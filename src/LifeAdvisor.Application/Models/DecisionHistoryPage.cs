namespace LifeAdvisor.Application.Models;

// A single page of decision history results plus the paging/filter context
// needed to render pagination and keep the active search/filter sticky.
public class DecisionHistoryPage
{
    public IReadOnlyList<DecisionHistoryItem> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
