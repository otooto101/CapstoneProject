using LifeAdvisor.Domain.Entities;

namespace LifeAdvisor.Application.Interfaces.Repositories;

public interface ITwinInterestRepository : IRepository<TwinInterest>
{
    Task<TwinInterest?> GetByTwinAndTopicAsync(int digitalTwinId, int interestTopicId, CancellationToken ct = default);
    Task<IReadOnlyList<TwinInterest>> ListInterestedByUserAsync(string identityUserId, CancellationToken ct = default);
}
