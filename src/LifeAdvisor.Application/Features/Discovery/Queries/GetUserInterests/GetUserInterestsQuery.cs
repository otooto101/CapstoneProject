using MediatR;

namespace LifeAdvisor.Application.Features.Discovery.Queries.GetUserInterests;

public record GetUserInterestsQuery(string IdentityUserId) : IRequest<IReadOnlyList<UserInterestDto>>;
