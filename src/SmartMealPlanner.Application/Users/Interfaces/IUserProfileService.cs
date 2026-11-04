using SmartMealPlanner.Application.Users.DTOs;

namespace SmartMealPlanner.Application.Users.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserProfileDto> UpdateMyProfileAsync(
        Guid userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default);
}
