using LifeAdvisor.Application.Interfaces.Repositories;
using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Queries.GetSwipeDeck;

public class GetSwipeDeckQueryHandler(IInterestTopicRepository interestTopicRepository)
    : IRequestHandler<GetSwipeDeckQuery, IReadOnlyList<SwipeTopicDto>>
{
    public async Task<IReadOnlyList<SwipeTopicDto>> Handle(GetSwipeDeckQuery request, CancellationToken ct)
    {
        var topics = await interestTopicRepository.ListUnseenForUserAsync(request.IdentityUserId, ct);

        return topics
            .Select(topic => new SwipeTopicDto(
                topic.InterestTopicId,
                topic.Title,
                topic.Description,
                topic.Category,
                topic.Emoji,
                topic.ImageUrl))
            .ToList();
    }
}
