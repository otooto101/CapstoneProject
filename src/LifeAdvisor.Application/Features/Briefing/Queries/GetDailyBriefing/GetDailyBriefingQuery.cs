using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;

public record GetDailyBriefingQuery(string IdentityUserId) : IRequest<DailyBriefingDto>;
