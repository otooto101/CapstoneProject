using LifeAdvisor.Application.Interfaces;
using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Domain.Entities;
using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Commands.SaveInterest;

public class SaveInterestCommandHandler(
    IDigitalTwinRepository digitalTwinRepository,
    IInterestTopicRepository interestTopicRepository,
    ITwinInterestRepository twinInterestRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveInterestCommand, int>
{
    public async Task<int> Handle(SaveInterestCommand request, CancellationToken ct)
    {
        var twin = await digitalTwinRepository.GetByIdentityUserIdAsync(request.IdentityUserId, ct)
            ?? throw new InvalidOperationException("Complete onboarding before saving your interests.");

        var topic = await interestTopicRepository.GetByIdAsync(request.TopicId, ct)
            ?? throw new InvalidOperationException("That topic could not be found.");

        var priority = request.IsInterested ? Math.Clamp(request.Priority, 1, 3) : 0;

        var existing = await twinInterestRepository.GetByTwinAndTopicAsync(twin.DigitalTwinId, topic.InterestTopicId, ct);

        if (existing is null)
        {
            await twinInterestRepository.AddAsync(new TwinInterest
            {
                DigitalTwinId = twin.DigitalTwinId,
                InterestTopicId = topic.InterestTopicId,
                IsInterested = request.IsInterested,
                Priority = priority
            }, ct);
        }
        else
        {
            existing.IsInterested = request.IsInterested;
            existing.Priority = priority;
            existing.UpdatedAt = DateTime.UtcNow;
            twinInterestRepository.Update(existing);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var remaining = await interestTopicRepository.ListUnseenForUserAsync(request.IdentityUserId, ct);
        return remaining.Count;
    }
}
