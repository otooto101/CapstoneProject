using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand : IRequest<int>
{
    public string IdentityUserId { get; init; } = string.Empty;
    public string PreferredName { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int LifeStageOptionId { get; init; }
}
