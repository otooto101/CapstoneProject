using System.Collections.Generic;
using LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;
using LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;

namespace LifeAdvisor.Web.Models;

public class InterestsViewModel : DashboardShellViewModel
{
    public IReadOnlyList<UserInterestDto> Interests { get; set; } = new List<UserInterestDto>();

    // Today's briefing, so the "Your updates" feed shows real stories instead of placeholders.
    public DailyBriefingDto? Briefing { get; set; }
}
