using LifeAdvisor.Application.Models;

namespace LifeAdvisor.Web.Models;

public class DecisionHistoryViewModel : DashboardShellViewModel
{
    public IReadOnlyList<DecisionHistoryItem> Items { get; set; } = [];
}