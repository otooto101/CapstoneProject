using LifeAdvisor.Application.Features.Auth.Commands.Login;
using LifeAdvisor.Application.Features.DigitalTwins.Commands.RegisterDigitalTwin;
using LifeAdvisor.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
            var command = new RegisterDigitalTwinCommand
            {
                Email = model.Email,
                Password = model.Password,
                PreferredName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                City = model.City,
                Country = model.Country,
                LifeStageOptionId = 1
            };

            await sender.Send(command, ct);
            return RedirectToAction(nameof(Login));
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
            await sender.Send(new LoginCommand(model.Email, model.Password), ct);
            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
