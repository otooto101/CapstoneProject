using LifeAdvisor.Application.Interfaces;
using MediatR;

namespace LifeAdvisor.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, string>
{
    public async Task<string> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request.Email, request.Password, ct);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors));

        return result.UserId!;
    }
}
