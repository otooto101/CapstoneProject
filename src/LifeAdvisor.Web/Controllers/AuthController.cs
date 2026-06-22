using LifeAdvisor.Application.Features.Auth.Commands.Login;
using LifeAdvisor.Application.Features.Auth.Commands.RegisterUser;
using LifeAdvisor.Application.Features.DigitalTwins.Commands.CompleteOnboarding;
using LifeAdvisor.Application.Features.DigitalTwins.Queries.GetLifeStageOptions;
using LifeAdvisor.Application.Features.DigitalTwins.Queries.GetOnboardingStatus;
using LifeAdvisor.Web.Infrastructure;
using LifeAdvisor.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LifeAdvisor.Web.Controllers;
public class AuthController(ISender sender) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = await sender.Send(new RegisterUserCommand(model.Email, model.Password), ct);
            HttpContext.Session.SetString(WebSessionKeys.CurrentUserId, userId);
            HttpContext.Session.SetString(WebSessionKeys.CurrentUserEmail, model.Email);
            HttpContext.Session.SetString(WebSessionKeys.CurrentUserDisplayName, model.Email.Split('@')[0]);

            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Onboarding(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId);

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(Login));

        var onboardingStatus = await sender.Send(new GetOnboardingStatusQuery(userId), ct);

        if (onboardingStatus.IsCompleted)
            return RedirectToAction("Index", "Home");

        var model = new OnboardingViewModel();
        await PopulateLifeStageOptionsAsync(model, ct);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Onboarding(OnboardingViewModel model, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetString(WebSessionKeys.CurrentUserId);

        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(Login));

        await PopulateLifeStageOptionsAsync(model, ct);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await sender.Send(new CompleteOnboardingCommand
            {
                IdentityUserId = userId,
                PreferredName = model.PreferredName,
                DateOfBirth = model.DateOfBirth,
                City = model.City,
                Country = model.Country,
                LifeStageOptionId = model.LifeStageOptionId
            }, ct);

            HttpContext.Session.SetString(WebSessionKeys.CurrentUserDisplayName, model.PreferredName);

            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var loginResult = await sender.Send(new LoginCommand(model.Email, model.Password), ct);

            HttpContext.Session.SetString(WebSessionKeys.CurrentUserId, loginResult.UserId);
            HttpContext.Session.SetString(WebSessionKeys.CurrentUserEmail, loginResult.Email);

            var onboardingStatus = await sender.Send(new GetOnboardingStatusQuery(loginResult.UserId), ct);
            HttpContext.Session.SetString(
                WebSessionKeys.CurrentUserDisplayName,
                onboardingStatus.IsCompleted
                    ? HttpContext.Session.GetString(WebSessionKeys.CurrentUserDisplayName) ?? loginResult.Email.Split('@')[0]
                    : loginResult.Email.Split('@')[0]);

            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private async Task PopulateLifeStageOptionsAsync(OnboardingViewModel model, CancellationToken ct)
    {
        var options = await sender.Send(new GetLifeStageOptionsQuery(), ct);

        model.LifeStageOptions = options
            .Select(option => new SelectListItem(option.Name, option.Id.ToString()))
            .ToList();

        if (model.LifeStageOptions.Count == 0)
            ModelState.AddModelError(string.Empty, "No life stage options are configured yet.");
    }
}
