using System.Text.Json;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using MediatR;

namespace LifeAdvisor.Application.Features.Analysis.Commands.CompleteDecision;

public class CompleteDecisionCommandHandler(
    ITwinNarrativeRepository twinNarrativeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteDecisionCommand>
{
    public async Task Handle(CompleteDecisionCommand request, CancellationToken ct)
    {
        var decision = await twinNarrativeRepository.GetDecisionByIdAsync(request.DecisionHistoryId, request.IdentityUserId, ct);
        if (decision is null)
            throw new InvalidOperationException("The decision history item could not be found.");

        var options = string.IsNullOrWhiteSpace(decision.ScenarioOptionsJson)
            ? []
            : JsonSerializer.Deserialize<List<DecisionScenarioOption>>(decision.ScenarioOptionsJson) ?? [];

        var selectedOption = options.FirstOrDefault(option => string.Equals(option.Title, request.SelectedScenarioTitle, StringComparison.Ordinal));
        if (selectedOption is null)
            throw new InvalidOperationException("Please choose one of the suggested scenarios.");

        decision.IsCompletedDecision = true;
        decision.SelectedScenarioTitle = selectedOption.Title;
        decision.UpdatedAt = DateTime.UtcNow;

        twinNarrativeRepository.Update(decision);
        await unitOfWork.SaveChangesAsync(ct);
    }
}