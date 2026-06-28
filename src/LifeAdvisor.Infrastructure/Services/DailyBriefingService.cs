using System.Text;
using System.Text.RegularExpressions;
using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LifeAdvisor.Infrastructure.Services;

// Composes a personalized daily briefing: the user's swipe interests decide what news to
// pull, real headlines come from INewsService, and the twin (Gemini) writes the greeting +
// a per-item "why this matters to you" line. Result is cached one-per-twin-per-day.
public class DailyBriefingService(
    IDigitalTwinRepository digitalTwinRepository,
    ITwinInterestRepository twinInterestRepository,
    IDailyBriefingRepository dailyBriefingRepository,
    INewsService newsService,
    GoogleNewsRssService worldNewsService,
    IUnitOfWork unitOfWork,
    SemanticKernelFactory semanticKernelFactory) : IDailyBriefingService
{
    private const int MaxItems = 6;
    private const int WorldSlots = 2;
    private const int MaxSearchTerms = 4;
    private const string GeneralTag = "Stay informed";
    private const string EssentialNewsQuery = "world OR politics OR economy OR election OR conflict";

    public async Task<DailyBriefing> GetOrCreateTodayAsync(string identityUserId, CancellationToken ct = default)
    {
        var twin = await RequireTwinAsync(identityUserId, ct);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var existing = await dailyBriefingRepository.GetForTwinOnDateAsync(twin.DigitalTwinId, today, ct);
        if (existing is not null)
            return existing;

        return await GenerateAndMaybePersistAsync(twin, today, ct);
    }

    public async Task<DailyBriefing> RegenerateTodayAsync(string identityUserId, CancellationToken ct = default)
    {
        var twin = await RequireTwinAsync(identityUserId, ct);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var existing = await dailyBriefingRepository.GetForTwinOnDateAsync(twin.DigitalTwinId, today, ct);
        if (existing is not null)
        {
            dailyBriefingRepository.Delete(existing);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return await GenerateAndMaybePersistAsync(twin, today, ct);
    }

    private async Task<DigitalTwin> RequireTwinAsync(string identityUserId, CancellationToken ct)
        => await digitalTwinRepository.GetByIdentityUserIdAsync(identityUserId, ct)
           ?? throw new InvalidOperationException("Finish onboarding first so your twin has something to brief you on.");

    private async Task<DailyBriefing> GenerateAndMaybePersistAsync(DigitalTwin twin, DateOnly today, CancellationToken ct)
    {
        var interests = await twinInterestRepository.ListInterestedByUserAsync(twin.IdentityUserId, ct);

        // Personalized stories come from the primary source — GNews (with photos) when a key is
        // set — and cost a single request. World/politics awareness comes from keyless Google
        // News RSS, which has no key, no quota, and no rate-limit cost, so we never spend the
        // photo-API budget just to keep them connected to the world.
        var interestQuery = BuildSearchQuery(interests);
        var interestArticles = string.IsNullOrEmpty(interestQuery)
            ? new List<Application.Models.NewsArticle>()
            : (await newsService.SearchAsync(interestQuery, MaxItems, ct)).ToList();

        var worldArticles = (await worldNewsService.SearchAsync(EssentialNewsQuery, WorldSlots + 2, ct)).ToList();

        // Interests lead (they're the point); the world slice fills reserved spots; then we
        // backfill with any remaining interest stories. Deduped by URL.
        var articles = BuildArticleMix(interestArticles, worldArticles);

        // No real news available (no key, no interests, or a provider hiccup): hand back a
        // tasteful, un-persisted briefing so the page renders and we retry next time.
        if (articles.Count == 0)
            return BuildFallbackBriefing(twin, interests);

        var synthesis = await SynthesizeWithTwinAsync(twin, interests, articles, ct);

        var briefing = new DailyBriefing
        {
            DigitalTwinId = twin.DigitalTwinId,
            BriefingDate = today,
            Greeting = synthesis.Greeting,
            GeneratedAt = DateTime.UtcNow
        };

        var order = 0;
        foreach (var article in articles)
        {
            briefing.Items.Add(new BriefingItem
            {
                Headline = article.Title,
                Blurb = article.Description,
                WhyItMatters = synthesis.WhyByIndex.TryGetValue(order, out var why) ? why : string.Empty,
                SourceName = article.SourceName,
                Url = article.Url,
                ImageUrl = article.ImageUrl,
                MatchedInterest = MatchInterest(article.Title, interests),
                PublishedAt = article.PublishedAt,
                DisplayOrder = order
            });
            order++;
        }

        await dailyBriefingRepository.AddAsync(briefing, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return briefing;
    }

    // Interests first (up to MaxItems - WorldSlots), then the world slice fills reserved spots,
    // then any leftover interest stories backfill. Deduped by URL, capped at MaxItems.
    private static List<Application.Models.NewsArticle> BuildArticleMix(
        IReadOnlyList<Application.Models.NewsArticle> interestArticles,
        IReadOnlyList<Application.Models.NewsArticle> worldArticles)
    {
        var picked = new List<Application.Models.NewsArticle>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Take(IEnumerable<Application.Models.NewsArticle> source, int upTo)
        {
            foreach (var article in source)
            {
                if (picked.Count >= upTo)
                    break;
                if (!string.IsNullOrWhiteSpace(article.Url) && seen.Add(article.Url))
                    picked.Add(article);
            }
        }

        Take(interestArticles, MaxItems - WorldSlots);
        Take(worldArticles, MaxItems);
        Take(interestArticles, MaxItems);
        return picked;
    }

    // One combined OR-query from the user's top interests, so a generation costs a single
    // request against the news quota.
    private static string BuildSearchQuery(IReadOnlyList<TwinInterest> interests)
    {
        var terms = interests
            .Select(i => Sanitize(i.Topic.Title))
            .Where(t => t.Length > 0)
            .Distinct()
            .Take(MaxSearchTerms)
            .Select(t => $"\"{t}\"");

        return string.Join(" OR ", terms);
    }

    private static string Sanitize(string title)
    {
        var cleaned = Regex.Replace(title, "[^a-zA-Z0-9 ]", " ");
        return Regex.Replace(cleaned, "\\s+", " ").Trim();
    }

    private static string MatchInterest(string articleTitle, IReadOnlyList<TwinInterest> interests)
    {
        foreach (var interest in interests)
        {
            foreach (var word in Sanitize(interest.Topic.Title).Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (word.Length >= 4 && articleTitle.Contains(word, StringComparison.OrdinalIgnoreCase))
                    return interest.Topic.Title;
            }
        }

        // No interest keyword hit — this is general/world news they should know regardless.
        return GeneralTag;
    }

    private async Task<BriefingSynthesis> SynthesizeWithTwinAsync(
        DigitalTwin twin,
        IReadOnlyList<TwinInterest> interests,
        IReadOnlyList<Application.Models.NewsArticle> articles,
        CancellationToken ct)
    {
        var name = string.IsNullOrWhiteSpace(twin.PreferredName) ? "there" : twin.PreferredName.Trim();
        var partOfDay = PartOfDay();
        var interestList = interests.Count > 0
            ? string.Join(", ", interests.Take(5).Select(i => i.Topic.Title))
            : "the world around them";

        try
        {
            var prompt = new StringBuilder();
            prompt.AppendLine($"You are {name}'s personal digital twin. You tell them the truth plainly — no sugarcoating, no hype — like someone who respects them enough to be straight.");
            prompt.AppendLine($"{name} cares about: {interestList}.");
            prompt.AppendLine($"It is {partOfDay} for them.");
            prompt.AppendLine($"Write a 2-sentence greeting that addresses {name} by name and fits the time of day. Be calm and matter-of-fact — state plainly what's worth their attention, no fluff and no cheerleading.");
            prompt.AppendLine("Then, for each numbered headline below, write ONE sentence (max 24 words, second person 'you') giving your honest take on why it matters and what's at stake. Be plain and concrete — never reassuring filler, never alarmist.");
            prompt.AppendLine("Write in plain text only. Never use emojis anywhere.");
            prompt.AppendLine("Return EXACTLY this format and nothing else:");
            prompt.AppendLine("GREETING: <greeting>");
            prompt.AppendLine("1: <why it matters>");
            prompt.AppendLine("2: <why it matters>");
            prompt.AppendLine("(one numbered line per headline)");
            prompt.AppendLine();
            prompt.AppendLine("Headlines:");
            for (var i = 0; i < articles.Count; i++)
                prompt.AppendLine($"{i + 1}. {articles[i].Title} — {articles[i].Description}");

            var kernel = semanticKernelFactory.CreateKernel();
            var chat = kernel.Services.GetRequiredService<IChatCompletionService>();
            var response = await chat.GetChatMessageContentAsync(prompt.ToString(), cancellationToken: ct);

            return ParseSynthesis(response.Content ?? string.Empty, name, partOfDay, interestList, articles.Count);
        }
        catch
        {
            // Gemini unavailable: we still have real headlines, so ship a clean greeting
            // and let the blurbs speak for themselves.
            return new BriefingSynthesis(FallbackGreeting(name, partOfDay, interestList), new Dictionary<int, string>());
        }
    }

    private static BriefingSynthesis ParseSynthesis(string content, string name, string partOfDay, string interestList, int itemCount)
    {
        var greeting = string.Empty;
        var whyByIndex = new Dictionary<int, string>();

        foreach (var raw in content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (raw.StartsWith("GREETING:", StringComparison.OrdinalIgnoreCase))
            {
                greeting = raw[9..].Trim();
                continue;
            }

            var match = Regex.Match(raw, "^(\\d+)\\s*[:.)-]\\s*(.+)$");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var num) && num >= 1 && num <= itemCount)
                whyByIndex[num - 1] = match.Groups[2].Value.Trim();
        }

        if (string.IsNullOrWhiteSpace(greeting))
            greeting = FallbackGreeting(name, partOfDay, interestList);

        return new BriefingSynthesis(greeting, whyByIndex);
    }

    private DailyBriefing BuildFallbackBriefing(DigitalTwin twin, IReadOnlyList<TwinInterest> interests)
    {
        var name = string.IsNullOrWhiteSpace(twin.PreferredName) ? "there" : twin.PreferredName.Trim();
        var partOfDay = PartOfDay();
        var interestList = interests.Count > 0
            ? string.Join(", ", interests.Take(3).Select(i => i.Topic.Title))
            : "the stories worth your time";

        return new DailyBriefing
        {
            DigitalTwinId = twin.DigitalTwinId,
            BriefingDate = DateOnly.FromDateTime(DateTime.Now),
            Greeting = FallbackGreeting(name, partOfDay, interestList),
            GeneratedAt = DateTime.UtcNow,
            Items = new List<BriefingItem>()
        };
    }

    private static string FallbackGreeting(string name, string partOfDay, string interestList)
        => $"{name}, it's {partOfDay}. Here's what's actually worth your attention on {interestList} — straight, no filler.";

    private static string PartOfDay()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            < 12 => "morning",
            < 17 => "afternoon",
            _ => "evening"
        };
    }

    private sealed record BriefingSynthesis(string Greeting, IReadOnlyDictionary<int, string> WhyByIndex);
}
