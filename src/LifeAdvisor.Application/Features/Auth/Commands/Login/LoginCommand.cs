using MediatR;

namespace LifeAdvisor.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<string>;
