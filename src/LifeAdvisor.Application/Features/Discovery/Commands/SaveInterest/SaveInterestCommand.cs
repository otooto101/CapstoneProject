using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Commands.SaveInterest;

// Returns how many topics are still left in the user's swipe deck.
public record SaveInterestCommand(string IdentityUserId, int TopicId, bool IsInterested, int Priority) : IRequest<int>;
