using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Commands.RegisterDigitalTwin;

public record RegisterDigitalTwinCommand : IRequest<int>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PreferredName { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int LifeStageOptionId { get; init; }
}
