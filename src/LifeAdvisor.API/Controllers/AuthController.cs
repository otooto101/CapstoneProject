using LifeAdvisor.Application.Features.Auth.Commands.Login;
using LifeAdvisor.Application.Features.DigitalTwins.Commands.RegisterDigitalTwin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LifeAdvisor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDigitalTwinCommand command, CancellationToken ct)
    {
        try
        {
            var id = await sender.Send(command, ct);
            return CreatedAtAction(nameof(Register), new { id }, new { DigitalTwinId = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Errors = ex.Message.Split("; ") });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        try
        {
            var result = await sender.Send(command, ct);
            return Ok(new { Token = result.Token, UserId = result.UserId, Email = result.Email });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Errors = ex.Message.Split("; ") });
        }
    }
}
