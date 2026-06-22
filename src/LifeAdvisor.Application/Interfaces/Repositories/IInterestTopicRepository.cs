using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces.Repositories;

public interface IInterestTopicRepository : IRepository<InterestTopic>
{
    // Active topics the user hasn't swiped on yet (their swipe deck).
    Task<IReadOnlyList<InterestTopic>> ListUnseenForUserAsync(string identityUserId, CancellationToken ct = default);
}
