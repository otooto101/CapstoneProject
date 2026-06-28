using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Web.Models;

public class DecisionHistoryViewModel : DashboardShellViewModel
{
    public IReadOnlyList<DecisionHistoryItem> Items { get; set; } = [];

    public string? Search { get; set; }

    // "all" | "completed" | "uncompleted"
    public string StatusFilter { get; set; } = "all";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}