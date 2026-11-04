using SmartMealPlanner.Application.Users.Models;

namespace SmartMealPlanner.Application.Users.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfileDetail?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserProfileDetail?> UpdateAsync(
        Guid userId,
        string displayName,
        string? avatarUrl,
        IReadOnlyList<string> dietaryPreferences,
        int defaultServings,
        string timezone,
        CancellationToken cancellationToken = default);
}
