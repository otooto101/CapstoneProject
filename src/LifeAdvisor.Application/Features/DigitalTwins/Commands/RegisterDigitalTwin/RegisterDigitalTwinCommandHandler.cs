using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using LifeAdvisor.Domain.ValueObjects;
using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Commands.RegisterDigitalTwin;

public class RegisterDigitalTwinCommandHandler(
    IAuthService authService,
    IDigitalTwinRepository twinRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterDigitalTwinCommand, int>
{
    public async Task<int> Handle(RegisterDigitalTwinCommand request, CancellationToken ct)
    {
        var authResult = await authService.RegisterAsync(request.Email, request.Password, ct);

        if (!authResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", authResult.Errors));

        try
        {
            var twin = new DigitalTwin
            {
                IdentityUserId = authResult.UserId!,
                PreferredName = request.PreferredName,
                DateOfBirth = request.DateOfBirth,
                Location = new Address(request.City, request.Country),
                LifeStageOptionId = request.LifeStageOptionId
            };

            await twinRepository.AddAsync(twin, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return twin.DigitalTwinId;
        }
        catch
        {
            await authService.DeleteUserAsync(authResult.UserId!, ct);
            throw;
        }
    }
}
