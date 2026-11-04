using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Application.Users.Models;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfileRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<UserProfileDetail?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .Include(p => p.Household)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return null;
        }

        return await MapToDetailAsync(profile, cancellationToken);
    }

    public async Task<UserProfileDetail?> UpdateAsync(
        Guid userId,
        string displayName,
        string? avatarUrl,
        IReadOnlyList<string> dietaryPreferences,
        int defaultServings,
        string timezone,
        CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.UserProfiles
            .Include(p => p.Household)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return null;
        }

        profile.DisplayName = displayName;
        profile.AvatarUrl = avatarUrl;
        profile.DietaryPreferences = dietaryPreferences.ToList();
        profile.DefaultServings = defaultServings;
        profile.Timezone = timezone;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await MapToDetailAsync(profile, cancellationToken);
    }

    private async Task<UserProfileDetail> MapToDetailAsync(
        Domain.Entities.UserProfile profile,
        CancellationToken cancellationToken)
    {
        var membership = await _dbContext.HouseholdMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.UserId == profile.UserId && m.HouseholdId == profile.HouseholdId,
                cancellationToken);

        var user = await _userManager.FindByIdAsync(profile.UserId.ToString());

        return new UserProfileDetail
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Email = user?.Email ?? string.Empty,
            DisplayName = profile.DisplayName,
            AvatarUrl = profile.AvatarUrl,
            DietaryPreferences = profile.DietaryPreferences,
            DefaultServings = profile.DefaultServings,
            Timezone = profile.Timezone,
            HouseholdId = profile.HouseholdId,
            HouseholdName = profile.Household.Name,
            HouseholdRole = membership?.Role == HouseholdRole.Owner ? "owner" : "member",
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
