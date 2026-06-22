using MediatR;

namespace LifeAdvisor.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest<string>;
