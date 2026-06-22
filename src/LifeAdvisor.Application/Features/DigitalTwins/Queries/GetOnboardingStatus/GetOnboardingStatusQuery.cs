using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Queries.GetOnboardingStatus;

public record GetOnboardingStatusQuery(string IdentityUserId) : IRequest<OnboardingStatusDto>;
