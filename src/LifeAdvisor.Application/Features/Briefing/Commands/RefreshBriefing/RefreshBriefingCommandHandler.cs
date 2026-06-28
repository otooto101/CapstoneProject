using LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;
using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Commands.RefreshBriefing;

public class RefreshBriefingCommandHandler(IDailyBriefingService dailyBriefingService)
    : IRequestHandler<RefreshBriefingCommand, DailyBriefingDto>
{
    public async Task<DailyBriefingDto> Handle(RefreshBriefingCommand request, CancellationToken ct)
    {
        var briefing = await dailyBriefingService.RegenerateTodayAsync(request.IdentityUserId, ct);
        return DailyBriefingDto.From(briefing);
    }
}
