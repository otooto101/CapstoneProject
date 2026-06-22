using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Queries.GetLifeStageOptions;

public record GetLifeStageOptionsQuery : IRequest<IReadOnlyList<LifeStageOptionDto>>;
