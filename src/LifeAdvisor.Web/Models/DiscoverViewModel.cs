using System.Collections.Generic;
using LifeAdvisor.Application.Features.Discovery.Queries.GetSwipeDeck;

namespace LifeAdvisor.Web.Models;

public class DiscoverViewModel : DashboardShellViewModel
{
    public IReadOnlyList<SwipeTopicDto> Topics { get; set; } = new List<SwipeTopicDto>();
}
