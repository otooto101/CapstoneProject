using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Briefing.Queries.GetDailyBriefing;

public class GetDailyBriefingQueryHandler(IDailyBriefingService dailyBriefingService)
    : IRequestHandler<GetDailyBriefingQuery, DailyBriefingDto>
{
    public async Task<DailyBriefingDto> Handle(GetDailyBriefingQuery request, CancellationToken ct)
    {
        var briefing = await dailyBriefingService.GetOrCreateTodayAsync(request.IdentityUserId, ct);
        return DailyBriefingDto.From(briefing);
    }
}
