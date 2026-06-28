using LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;

namespace LifeAdvisor.Web.Models;

public class HomeDashboardViewModel : DashboardShellViewModel
{
    // Today's personalized briefing — null when the user hasn't onboarded yet or it
    // couldn't be generated.
    public DailyBriefingDto? Briefing { get; set; }
}
