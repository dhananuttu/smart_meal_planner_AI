using SmartMealPlanner.Application.Users.DTOs;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Application.Users.Models;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Users.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User profile", userId);

        return MapToDto(profile);
    }

    public async Task<UserProfileDto> UpdateMyProfileAsync(
        Guid userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedPreferences = request.DietaryPreferences
            .Select(p => p.Trim().ToLowerInvariant())
            .Where(p => p.Length > 0)
            .Distinct()
            .ToList();

        var profile = await _userProfileRepository.UpdateAsync(
            userId,
            request.DisplayName.Trim(),
            string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim(),
            normalizedPreferences,
            request.DefaultServings,
            request.Timezone.Trim(),
            cancellationToken)
            ?? throw new NotFoundException("User profile", userId);

        return MapToDto(profile);
    }

    private static UserProfileDto MapToDto(UserProfileDetail profile) =>
        new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Email = profile.Email,
            DisplayName = profile.DisplayName,
            AvatarUrl = profile.AvatarUrl,
            DietaryPreferences = profile.DietaryPreferences,
            DefaultServings = profile.DefaultServings,
            Timezone = profile.Timezone,
            HouseholdId = profile.HouseholdId,
            HouseholdName = profile.HouseholdName,
            HouseholdRole = profile.HouseholdRole,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
}
