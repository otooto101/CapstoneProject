using LifeAdvisor.Application.Interfaces.Repositories;
using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Queries.GetLifeStageOptions;

public class GetLifeStageOptionsQueryHandler(ILifeStageOptionRepository lifeStageOptionRepository)
    : IRequestHandler<GetLifeStageOptionsQuery, IReadOnlyList<LifeStageOptionDto>>
{
    public async Task<IReadOnlyList<LifeStageOptionDto>> Handle(GetLifeStageOptionsQuery request, CancellationToken ct)
    {
        var options = await lifeStageOptionRepository.ListAsync(option => option.IsActive, ct);

        return options
            .OrderBy(option => option.DisplayOrder)
            .ThenBy(option => option.Name)
            .Select(option => new LifeStageOptionDto(option.LifeStageOptionId, option.Name))
            .ToList();
    }
}
