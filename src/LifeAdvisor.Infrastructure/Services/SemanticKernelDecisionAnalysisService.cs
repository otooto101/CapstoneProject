using System.Text;
using System.Text.Json;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Entities;
using LifeAdvisor.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LifeAdvisor.Infrastructure.Services;

public class SemanticKernelDecisionAnalysisService(
    IAnalysisSettingsService analysisSettingsService,
    IRelatedDecisionRetriever relatedDecisionRetriever,
    ITextEmbeddingService textEmbeddingService,
    ITwinNarrativeRepository twinNarrativeRepository,
    IDigitalTwinRepository digitalTwinRepository,
    IUnitOfWork unitOfWork,
    SemanticKernelFactory semanticKernelFactory) : IDecisionAnalysisService
{
    public async Task<DecisionAnalysisResult> AnalyzeAsync(string identityUserId, string prompt, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new InvalidOperationException("Please describe the decision you want to analyze.");

        try
        {
            var settings = await analysisSettingsService.GetSettingsAsync(identityUserId, ct);
            var queryEmbedding = await textEmbeddingService.GenerateAsync(prompt, ct);
            var relatedDecisions = await relatedDecisionRetriever.RetrieveAsync(
                identityUserId,
                queryEmbedding,
                settings.MaxRelatedDecisions,
                settings.SimilarityThreshold,
                ct);

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("You are a thoughtful Digital Twin that helps a person reflect on difficult decisions.");
            promptBuilder.AppendLine("Be empathetic, practical, and specific. Do not judge.");
            promptBuilder.AppendLine("Write for a real customer who needs clarity, emotional insight, and practical next steps.");
            promptBuilder.AppendLine("Structure the response with these section headers exactly: Situation, What Seems To Be Driving This, Risks To Watch, What Future You Might Thank You For, and A Grounded Next Step.");
            promptBuilder.AppendLine("Each section should be descriptive, supportive, and concrete rather than short or generic.");
            promptBuilder.AppendLine("After the guidance, provide exactly 3 scenario options. Each option must include a strong title, a numeric score out of 100, a 2-4 sentence summary, the best condition for choosing it, the main risk, and the first action step.");
            promptBuilder.AppendLine("Return using this exact format:");
            promptBuilder.AppendLine("GUIDANCE: <full guidance>");
            promptBuilder.AppendLine("SCENARIO 1: <title> | SCORE: <0-100> | SUMMARY: <summary> | BEST WHEN: <best condition> | RISK: <main risk> | FIRST STEP: <first action>");
            promptBuilder.AppendLine("SCENARIO 2: <title> | SCORE: <0-100> | SUMMARY: <summary> | BEST WHEN: <best condition> | RISK: <main risk> | FIRST STEP: <first action>");
            promptBuilder.AppendLine("SCENARIO 3: <title> | SCORE: <0-100> | SUMMARY: <summary> | BEST WHEN: <best condition> | RISK: <main risk> | FIRST STEP: <first action>");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Current decision to analyze:");
            promptBuilder.AppendLine(prompt.Trim());
            promptBuilder.AppendLine();

            if (relatedDecisions.Count > 0)
            {
                promptBuilder.AppendLine("Relevant prior decision history (use only if genuinely relevant):");
                foreach (var match in relatedDecisions)
                {
                    promptBuilder.AppendLine($"- [{match.Type}] score={match.SimilarityScore:F2}: {match.Content}");
                }
            }
            else
            {
                promptBuilder.AppendLine("No strongly related past decisions were found above the similarity threshold. Analyze only the current situation.");
            }

            var kernel = semanticKernelFactory.CreateKernel();
            var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();

            var response = await chatCompletionService.GetChatMessageContentAsync(
                promptBuilder.ToString(),
                cancellationToken: ct);

            var parsedResponse = ParseResponse(response.Content ?? string.Empty, prompt);
            var decisionRecord = await SavePendingDecisionAsync(identityUserId, prompt, parsedResponse.Scenarios, ct);

            return new DecisionAnalysisResult
            {
                DecisionHistoryId = decisionRecord.TwinNarrativeId,
                Prompt = prompt,
                HistoryAcknowledgement = "Your decision analysis will soon be added in history. Choose one path below when you are ready.",
                Guidance = parsedResponse.Guidance,
                ScenarioOptions = parsedResponse.Scenarios,
                RelatedDecisions = relatedDecisions,
                ConsideredDecisionCount = settings.MaxRelatedDecisions,
                SimilarityThreshold = settings.SimilarityThreshold
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("The AI analysis service is currently unavailable. Check your Gemini model configuration and try again.", ex);
        }
    }

    public async Task CompleteDecisionAsync(string identityUserId, int decisionHistoryId, string selectedScenarioTitle, CancellationToken ct = default)
    {
        var decision = await twinNarrativeRepository.GetDecisionByIdAsync(decisionHistoryId, identityUserId, ct);
        if (decision is null)
            throw new InvalidOperationException("The decision history item could not be found.");

        var options = string.IsNullOrWhiteSpace(decision.ScenarioOptionsJson)
            ? []
            : JsonSerializer.Deserialize<List<DecisionScenarioOption>>(decision.ScenarioOptionsJson) ?? [];

        var selectedOption = options.FirstOrDefault(option => string.Equals(option.Title, selectedScenarioTitle, StringComparison.Ordinal));
        if (selectedOption is null)
            throw new InvalidOperationException("Please choose one of the suggested scenarios.");

        decision.IsCompletedDecision = true;
        decision.SelectedScenarioTitle = selectedOption.Title;
        decision.UpdatedAt = DateTime.UtcNow;

        twinNarrativeRepository.Update(decision);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DecisionHistoryItem>> GetDecisionHistoryAsync(string identityUserId, CancellationToken ct = default)
    {
        var history = await twinNarrativeRepository.ListDecisionHistoryAsync(identityUserId, ct);
        return history.Select(item => new DecisionHistoryItem
        {
            TwinNarrativeId = item.TwinNarrativeId,
            Title = string.IsNullOrWhiteSpace(item.DecisionTitle) ? item.Content : item.DecisionTitle,
            Prompt = item.Content,
            IsCompleted = item.IsCompletedDecision,
            SelectedScenarioTitle = item.SelectedScenarioTitle,
            ScenarioOptions = string.IsNullOrWhiteSpace(item.ScenarioOptionsJson)
                ? []
                : JsonSerializer.Deserialize<List<DecisionScenarioOption>>(item.ScenarioOptionsJson) ?? [],
            CreatedAt = item.CreatedAt
        }).ToList();
    }

    private async Task<TwinNarrative> SavePendingDecisionAsync(
        string identityUserId,
        string prompt,
        IReadOnlyList<DecisionScenarioOption> scenarios,
        CancellationToken ct)
    {
        var twin = await digitalTwinRepository.GetByIdentityUserIdAsync(identityUserId, ct)
            ?? throw new InvalidOperationException("You need to complete onboarding before saving decision history.");

        var narrative = new TwinNarrative
        {
            DigitalTwinId = twin.DigitalTwinId,
            Type = NarrativeType.CurrentStruggles,
            Content = prompt.Trim(),
            IsDecision = true,
            IsCompletedDecision = false,
            DecisionTitle = BuildDecisionTitle(prompt),
            ScenarioOptionsJson = JsonSerializer.Serialize(scenarios),
            UpdatedAt = DateTime.UtcNow
        };

        await twinNarrativeRepository.AddAsync(narrative, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return narrative;
    }

    private static string BuildDecisionTitle(string prompt)
        => prompt.Trim().Length <= 80 ? prompt.Trim() : $"{prompt.Trim()[..77]}...";

    private static ParsedAnalysisResponse ParseResponse(string content, string prompt)
    {
        var lines = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var guidanceBuilder = new StringBuilder();
        var scenarios = new List<DecisionScenarioOption>();

        foreach (var line in lines)
        {
            if (line.StartsWith("GUIDANCE:", StringComparison.OrdinalIgnoreCase))
            {
                guidanceBuilder.AppendLine(line[9..].Trim());
                continue;
            }

            if (line.StartsWith("SCENARIO", StringComparison.OrdinalIgnoreCase))
            {
                scenarios.Add(ParseScenario(line));
                continue;
            }

            if (scenarios.Count == 0)
                guidanceBuilder.AppendLine(line);
        }

        while (scenarios.Count < 3)
        {
            scenarios.Add(new DecisionScenarioOption
            {
                Title = scenarios.Count switch
                {
                    0 => "Move and Start Fresh",
                    1 => "Stay and Face What's Here",
                    _ => "Have the Conversation First"
                },
                Score = scenarios.Count switch
                {
                    0 => 75,
                    1 => 68,
                    _ => 92
                },
                Summary = scenarios.Count switch
                {
                    0 => "Take a short, structured pause instead of forcing a permanent exit. This gives you room to recover, think clearly, and test whether your stress is about the environment, burnout, or the path itself.",
                    1 => "Stay in place for now, but stop trying to carry this alone. A better support structure could change the experience more than a dramatic decision made under pressure.",
                    _ => "Before making a final decision, have one honest conversation with the people who can help you see options you may be missing. Clarity often comes faster when you stop holding the whole problem privately."
                },
                BestWhen = scenarios.Count switch
                {
                    0 => "you feel exhausted and need space without burning the bridge completely",
                    1 => "you still care about the degree and believe the main problem may be support, pressure, or mental overload",
                    _ => "you are torn and need more truth, guidance, or practical information before choosing"
                },
                MainRisk = scenarios.Count switch
                {
                    0 => "the pause becomes avoidance if you do not attach it to a clear review plan",
                    1 => "you keep suffering in the same pattern if nothing in your environment actually changes",
                    _ => "you delay too long and stay stuck between options without relief"
                },
                FirstStep = scenarios.Count switch
                {
                    0 => "Book a meeting with your advisor this week to ask what a leave of absence would actually look like.",
                    1 => "List the three biggest pressures and bring them to student support, a mentor, or a trusted academic advisor.",
                    _ => "Have one direct conversation in the next 48 hours with someone who understands your academic and personal reality."
                }
            });
        }

        return new ParsedAnalysisResponse(
            string.IsNullOrWhiteSpace(guidanceBuilder.ToString()) ? content : guidanceBuilder.ToString().Trim(),
            scenarios.Take(3).ToList());
    }

    private static DecisionScenarioOption ParseScenario(string line)
    {
        var parts = line.Split('|', StringSplitOptions.TrimEntries);
        var titlePart = parts.Length > 0 ? parts[0] : "Scenario";
        var scorePart = parts.FirstOrDefault(part => part.StartsWith("SCORE:", StringComparison.OrdinalIgnoreCase)) ?? "SCORE: 50";
        var summaryPart = parts.FirstOrDefault(part => part.StartsWith("SUMMARY:", StringComparison.OrdinalIgnoreCase)) ?? "SUMMARY: A grounded next move.";
        var bestWhenPart = parts.FirstOrDefault(part => part.StartsWith("BEST WHEN:", StringComparison.OrdinalIgnoreCase)) ?? "BEST WHEN: when this path fits your real emotional and practical needs.";
        var riskPart = parts.FirstOrDefault(part => part.StartsWith("RISK:", StringComparison.OrdinalIgnoreCase)) ?? "RISK: moving too quickly without enough support or clarity.";
        var firstStepPart = parts.FirstOrDefault(part => part.StartsWith("FIRST STEP:", StringComparison.OrdinalIgnoreCase)) ?? "FIRST STEP: take one concrete action within the next day.";

        var title = titlePart.Contains(':') ? titlePart[(titlePart.IndexOf(':') + 1)..].Trim() : titlePart.Trim();
        var scoreText = scorePart[(scorePart.IndexOf(':') + 1)..].Trim();
        _ = int.TryParse(scoreText, out var score);

        return new DecisionScenarioOption
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Scenario" : title,
            Score = Math.Clamp(score, 0, 100),
            Summary = summaryPart[(summaryPart.IndexOf(':') + 1)..].Trim(),
            BestWhen = bestWhenPart[(bestWhenPart.IndexOf(':') + 1)..].Trim(),
            MainRisk = riskPart[(riskPart.IndexOf(':') + 1)..].Trim(),
            FirstStep = firstStepPart[(firstStepPart.IndexOf(':') + 1)..].Trim()
        };
    }

    private sealed record ParsedAnalysisResponse(string Guidance, IReadOnlyList<DecisionScenarioOption> Scenarios);
}
