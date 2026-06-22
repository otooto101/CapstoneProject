using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence.Repositories;

public class InterestTopicRepository(TwinDbContext context)
    : BaseRepository<InterestTopic, TwinDbContext>(context), IInterestTopicRepository
{
    public async Task<IReadOnlyList<InterestTopic>> ListUnseenForUserAsync(string identityUserId, CancellationToken ct = default)
        => await context.InterestTopics
            .Where(topic => topic.IsActive
                && !topic.Interests.Any(interest => interest.Twin.IdentityUserId == identityUserId))
            .OrderBy(topic => topic.DisplayOrder)
            .ToListAsync(ct);
}
