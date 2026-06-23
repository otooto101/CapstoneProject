using System.Text.Json;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetDecisionHistory;

public class GetDecisionHistoryQueryHandler(ITwinNarrativeRepository twinNarrativeRepository)
    : IRequestHandler<GetDecisionHistoryQuery, DecisionHistoryPage>
{
    public async Task<DecisionHistoryPage> Handle(GetDecisionHistoryQuery request, CancellationToken ct)
    {
        var (rows, totalCount) = await twinNarrativeRepository.SearchDecisionHistoryAsync(
            request.IdentityUserId,
            request.Search,
            request.IsCompleted,
            request.Page,
            request.PageSize,
            ct);

        var items = rows.Select(row => new DecisionHistoryItem
        {
            TwinNarrativeId = row.TwinNarrativeId,
            Title = string.IsNullOrWhiteSpace(row.DecisionTitle) ? row.Content : row.DecisionTitle,
            Prompt = row.Content,
            IsCompleted = row.IsCompletedDecision,
            SelectedScenarioTitle = row.SelectedScenarioTitle,
            ScenarioOptions = DeserializeOptions(row.ScenarioOptionsJson),
            CreatedAt = row.CreatedAt
        }).ToList();

        return new DecisionHistoryPage
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page < 1 ? 1 : request.Page,
            PageSize = request.PageSize < 1 ? 10 : request.PageSize
        };
    }

    private static IReadOnlyList<DecisionScenarioOption> DeserializeOptions(string json)
        => string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<DecisionScenarioOption>>(json) ?? [];
}