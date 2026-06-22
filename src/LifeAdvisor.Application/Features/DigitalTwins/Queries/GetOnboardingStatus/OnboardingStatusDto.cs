namespace LifeAdvisor.Application.Features.DigitalTwins.Queries.GetOnboardingStatus;

public record OnboardingStatusDto(bool IsCompleted, int? DigitalTwinId);
