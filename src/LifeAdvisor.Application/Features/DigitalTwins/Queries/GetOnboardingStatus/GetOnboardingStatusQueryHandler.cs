using LifeAdvisor.Application.Interfaces.Repositories;
using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Queries.GetOnboardingStatus;

public class GetOnboardingStatusQueryHandler(IDigitalTwinRepository twinRepository)
    : IRequestHandler<GetOnboardingStatusQuery, OnboardingStatusDto>
{
    public async Task<OnboardingStatusDto> Handle(GetOnboardingStatusQuery request, CancellationToken ct)
    {
        var hasTwin = await twinRepository.AnyAsync(
            twin => twin.IdentityUserId == request.IdentityUserId,
            ct);

        return new OnboardingStatusDto(hasTwin, null);
    }
}
