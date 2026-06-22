using System.Collections.Generic;
using LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;

namespace LifeAdvisor.Web.Models;

public class InterestsViewModel : DashboardShellViewModel
{
    public IReadOnlyList<UserInterestDto> Interests { get; set; } = new List<UserInterestDto>();
}
