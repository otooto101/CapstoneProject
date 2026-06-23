using LifeAdvisor.Application.Features.Analysis.Commands.AnalyzeDecision;
using LifeAdvisor.Application.Features.Analysis.Commands.CompleteDecision;
using LifeAdvisor.Application.Features.Analysis.Commands.UpdateAnalysisSettings;
using LifeAdvisor.Application.Features.Analysis.Queries.GetDecisionHistory;
using LifeAdvisor.Application.Features.Analysis.Queries.GetAnalysisSettings;
using LifeAdvisor.Application.Features.DigitalTwins.Queries.GetOnboardingStatus;
using LifeAdvisor.Application.Features.Discovery.Commands.SaveInterest;
using LifeAdvisor.Application.Features.Discovery.Queries.GetSwipeDeck;
using LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;
using LifeAdvisor.Web.Infrastructure;
using LifeAdvisor.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeAdvisor.Web.Controllers;

public class HomeController(ISender sender) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = await BuildDashboardShellModelAsync(new HomeDashboardViewModel(), ct);
        return View(model);
    }

    [HttpGet("/terms")]
    public IActionResult Terms() => View();

    [HttpGet("/privacy")]
    public IActionResult Privacy() => View();

    [HttpGet]
    public async Task<IActionResult> Discover(CancellationToken ct)
    {
        var model = await RequireSignedInAsync(new DiscoverViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        if (model.IsOnboardingCompleted)
        {
            var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;
            model.Topics = await sender.Send(new GetSwipeDeckQuery(userId), ct);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SaveInterest(int topicId, bool isInterested, int priority, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId);
        if (string.IsNullOrWhiteSpace(userId))
            return Json(new { ok = false, error = "Please sign in again." });

        try
        {
            var remaining = await sender.Send(new SaveInterestCommand(userId, topicId, isInterested, priority), ct);
            return Json(new { ok = true, remaining });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { ok = false, error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Interests(CancellationToken ct)
    {
        var model = await RequireSignedInAsync(new InterestsViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;
        model.Interests = await sender.Send(new GetUserInterestsQuery(userId), ct);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Analyze(CancellationToken ct)
    {
        var model = await RequireSignedInAsync(new AnalyzeDecisionViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(AnalyzeDecisionViewModel model, CancellationToken ct)
    {
        var hydratedModel = await RequireSignedInAsync(model, ct);
        if (hydratedModel is null)
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
            return View(hydratedModel);

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;

        try
        {
            var result = await sender.Send(new AnalyzeDecisionCommand(userId, model.Prompt), ct);
            hydratedModel.DecisionHistoryId = result.DecisionHistoryId;
            hydratedModel.HistoryAcknowledgement = result.HistoryAcknowledgement;
            hydratedModel.Guidance = result.Guidance;
            hydratedModel.ScenarioOptions = result.ScenarioOptions;
            hydratedModel.RelatedDecisions = result.RelatedDecisions;
            hydratedModel.ConsideredDecisionCount = result.ConsideredDecisionCount;
            hydratedModel.SimilarityThreshold = result.SimilarityThreshold;

            return View(hydratedModel);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(hydratedModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompleteDecision(AnalyzeDecisionViewModel model, CancellationToken ct)
    {
        var hydratedModel = await RequireSignedInAsync(model, ct);
        if (hydratedModel is null)
            return RedirectToAction("Login", "Auth");

        if (model.DecisionHistoryId <= 0 || string.IsNullOrWhiteSpace(model.SelectedScenarioTitle))
        {
            ModelState.AddModelError(string.Empty, "Choose one scenario to complete this decision.");
            return View("Analyze", hydratedModel);
        }

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;

        try
        {
            await sender.Send(new CompleteDecisionCommand(userId, model.DecisionHistoryId, model.SelectedScenarioTitle), ct);
            TempData["DecisionCompleted"] = "Decision completed and saved in your history.";
            return RedirectToAction(nameof(History));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Analyze", hydratedModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> History(string? search, string? status, int page = 1, CancellationToken ct = default)
    {
        var model = await RequireSignedInAsync(new DecisionHistoryViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;

        var statusFilter = (status ?? "all").ToLowerInvariant();
        bool? isCompleted = statusFilter switch
        {
            "completed" => true,
            "uncompleted" => false,
            _ => null
        };

        if (page < 1)
            page = 1;

        var result = await sender.Send(
            new GetDecisionHistoryQuery(userId, search, isCompleted, page, model.PageSize), ct);

        model.Items = result.Items;
        model.TotalCount = result.TotalCount;
        model.Page = result.Page;
        model.PageSize = result.PageSize;
        model.Search = search;
        model.StatusFilter = statusFilter is "completed" or "uncompleted" ? statusFilter : "all";

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CompleteDecisionFromHistory(int decisionHistoryId, string selectedScenarioTitle, CancellationToken ct)
    {
        var model = await RequireSignedInAsync(new DecisionHistoryViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;

        if (decisionHistoryId <= 0 || string.IsNullOrWhiteSpace(selectedScenarioTitle))
        {
            TempData["DecisionHistoryError"] = "Choose one scenario before marking the decision as complete.";
            return RedirectToAction(nameof(History));
        }

        try
        {
            await sender.Send(new CompleteDecisionCommand(userId, decisionHistoryId, selectedScenarioTitle), ct);
            TempData["DecisionCompleted"] = "Decision completed and updated from history.";
            return RedirectToAction(nameof(History));
        }
        catch (InvalidOperationException ex)
        {
            TempData["DecisionHistoryError"] = ex.Message;
            return RedirectToAction(nameof(History));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Settings(CancellationToken ct)
    {
        var model = await RequireSignedInAsync(new AnalysisSettingsViewModel(), ct);
        if (model is null)
            return RedirectToAction("Login", "Auth");

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;
        var settings = await sender.Send(new GetAnalysisSettingsQuery(userId), ct);
        model.MaxRelatedDecisions = settings.MaxRelatedDecisions;
        model.SimilarityThreshold = settings.SimilarityThreshold;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(AnalysisSettingsViewModel model, CancellationToken ct)
    {
        var hydratedModel = await RequireSignedInAsync(model, ct);
        if (hydratedModel is null)
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
            return View(hydratedModel);

        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId)!;

        try
        {
            await sender.Send(new UpdateAnalysisSettingsCommand(userId, model.MaxRelatedDecisions, model.SimilarityThreshold), ct);
            ViewData["SettingsSaved"] = true;
            return View(hydratedModel);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(hydratedModel);
        }
    }

    private async Task<T> BuildDashboardShellModelAsync<T>(T model, CancellationToken ct) where T : DashboardShellViewModel
    {
        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId);
        var email = HttpContext.Session.GetString(WebSessionKeys.CurrentUserEmail);
        var displayName = HttpContext.Session.GetString(WebSessionKeys.CurrentUserDisplayName);

        model.IsSignedIn = !string.IsNullOrWhiteSpace(userId);
        model.Email = string.IsNullOrWhiteSpace(email) ? "alex@email.com" : email;
        model.DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? (!string.IsNullOrWhiteSpace(email) ? email.Split('@')[0] : "Alex")
            : displayName;
        model.Initial = model.DisplayName[..1].ToUpperInvariant();

        if (model.IsSignedIn)
        {
            var onboardingStatus = await sender.Send(new GetOnboardingStatusQuery(userId!), ct);
            model.IsOnboardingCompleted = onboardingStatus.IsCompleted;
        }

        return model;
    }

    private async Task<T?> RequireSignedInAsync<T>(T model, CancellationToken ct) where T : DashboardShellViewModel
    {
        var hydratedModel = await BuildDashboardShellModelAsync(model, ct);
        return hydratedModel.IsSignedIn ? hydratedModel : null;
    }
}
