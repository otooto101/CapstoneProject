using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using LifeAdvisor.Domain.ValueObjects;
using MediatR;

namespace LifeAdvisor.Application.Features.DigitalTwins.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(
    IDigitalTwinRepository twinRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteOnboardingCommand, int>
{
    public async Task<int> Handle(CompleteOnboardingCommand request, CancellationToken ct)
    {
        var twinAlreadyExists = await twinRepository.AnyAsync(
            twin => twin.IdentityUserId == request.IdentityUserId,
            ct);

        if (twinAlreadyExists)
            throw new InvalidOperationException("Onboarding is already completed for this account.");

        var twin = new DigitalTwin
        {
            IdentityUserId = request.IdentityUserId,
            PreferredName = request.PreferredName,
            DateOfBirth = request.DateOfBirth,
            Location = new Address(request.City, request.Country),
            LifeStageOptionId = request.LifeStageOptionId
        };

        await twinRepository.AddAsync(twin, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return twin.DigitalTwinId;
    }
}
