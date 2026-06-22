using System.Text.Json;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Queries.GetDecisionHistory;

public class GetDecisionHistoryQueryHandler(ITwinNarrativeRepository twinNarrativeRepository)
    : IRequestHandler<GetDecisionHistoryQuery, IReadOnlyList<DecisionHistoryItem>>
{
    public async Task<IReadOnlyList<DecisionHistoryItem>> Handle(GetDecisionHistoryQuery request, CancellationToken ct)
    {
        var history = await twinNarrativeRepository.ListDecisionHistoryAsync(request.IdentityUserId, ct);

        return history.Select(item => new DecisionHistoryItem
        {
            TwinNarrativeId = item.TwinNarrativeId,
            Title = string.IsNullOrWhiteSpace(item.DecisionTitle) ? item.Content : item.DecisionTitle,
            Prompt = item.Content,
            IsCompleted = item.IsCompletedDecision,
            SelectedScenarioTitle = item.SelectedScenarioTitle,
            ScenarioOptions = DeserializeOptions(item.ScenarioOptionsJson),
            CreatedAt = item.CreatedAt
        }).ToList();
    }

    private static IReadOnlyList<DecisionScenarioOption> DeserializeOptions(string json)
        => string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<DecisionScenarioOption>>(json) ?? [];
}