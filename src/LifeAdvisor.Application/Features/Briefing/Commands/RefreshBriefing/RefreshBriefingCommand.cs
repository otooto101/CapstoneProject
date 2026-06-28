using LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Commands.RefreshBriefing;

public record RefreshBriefingCommand(string IdentityUserId) : IRequest<DailyBriefingDto>;
