using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Queries.GetSwipeDeck;

public record GetSwipeDeckQuery(string IdentityUserId) : IRequest<IReadOnlyList<SwipeTopicDto>>;
