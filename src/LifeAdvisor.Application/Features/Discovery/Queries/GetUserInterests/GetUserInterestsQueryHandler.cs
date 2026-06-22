using LifeAdvisor.Application.Interfaces.Repositories;
using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;

public class GetUserInterestsQueryHandler(ITwinInterestRepository twinInterestRepository)
    : IRequestHandler<GetUserInterestsQuery, IReadOnlyList<UserInterestDto>>
{
    public async Task<IReadOnlyList<UserInterestDto>> Handle(GetUserInterestsQuery request, CancellationToken ct)
    {
        var interests = await twinInterestRepository.ListInterestedByUserAsync(request.IdentityUserId, ct);

        return interests
            .Select(interest => new UserInterestDto(
                interest.InterestTopicId,
                interest.Topic.Title,
                interest.Topic.Description,
                interest.Topic.Category,
                interest.Topic.Emoji,
                interest.Topic.ImageUrl,
                interest.Priority))
            .ToList();
    }
}
