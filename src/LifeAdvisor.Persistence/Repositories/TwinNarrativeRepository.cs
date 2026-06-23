using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAdvisor.Persistence.Repositories;

public class TwinNarrativeRepository(TwinDbContext context)
    : BaseRepository<TwinNarrative, TwinDbContext>(context), ITwinNarrativeRepository
{
    public async Task<IReadOnlyList<TwinNarrative>> ListByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .Where(narrative => narrative.Twin.IdentityUserId == identityUserId)
            .OrderByDescending(narrative => narrative.CreatedAt)
            .ToListAsync(ct);

    public async Task<TwinNarrative?> GetDecisionByIdAsync(int twinNarrativeId, string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .FirstOrDefaultAsync(
                narrative => narrative.TwinNarrativeId == twinNarrativeId
                    && narrative.Twin.IdentityUserId == identityUserId
                    && narrative.IsDecision,
                ct);

    public async Task<IReadOnlyList<TwinNarrative>> ListDecisionHistoryAsync(string identityUserId, CancellationToken ct = default)
        => await context.TwinNarratives
            .Include(narrative => narrative.Twin)
            .Where(narrative => narrative.Twin.IdentityUserId == identityUserId && narrative.IsDecision)
            .OrderByDescending(narrative => narrative.CreatedAt)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<DecisionHistoryRow> Rows, int TotalCount)> SearchDecisionHistoryAsync(
        string identityUserId,
        string? search,
        bool? isCompleted,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 10;

        var query = context.TwinNarratives
            .Where(narrative => narrative.Twin.IdentityUserId == identityUserId && narrative.IsDecision);

        if (isCompleted.HasValue)
            query = query.Where(narrative => narrative.IsCompletedDecision == isCompleted.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{EscapeLike(search.Trim())}%";
            query = query.Where(narrative =>
                EF.Functions.Like(narrative.DecisionTitle, pattern)
                || EF.Functions.Like(narrative.Content, pattern)
                || EF.Functions.Like(narrative.SelectedScenarioTitle!, pattern));
        }

        var totalCount = await query.CountAsync(ct);

        var rows = await query
            .OrderByDescending(narrative => narrative.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(narrative => new DecisionHistoryRow
            {
                TwinNarrativeId = narrative.TwinNarrativeId,
                DecisionTitle = narrative.DecisionTitle,
                Content = narrative.Content,
                IsCompletedDecision = narrative.IsCompletedDecision,
                SelectedScenarioTitle = narrative.SelectedScenarioTitle,
                ScenarioOptionsJson = narrative.ScenarioOptionsJson,
                CreatedAt = narrative.CreatedAt
            })
            .ToListAsync(ct);

        return (rows, totalCount);
    }

    private static string EscapeLike(string input)
        => input
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("_", "[_]");
}
