using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Common.Interfaces;

public interface IHouseholdContext
{
    Guid UserId { get; }

    Guid HouseholdId { get; }

    HouseholdRole Role { get; }

    bool IsOwner { get; }

    Task EnsureMemberAsync(CancellationToken cancellationToken = default);

    Task EnsureOwnerAsync(CancellationToken cancellationToken = default);
}
