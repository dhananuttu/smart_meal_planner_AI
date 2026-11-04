using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Domain.Exceptions;
using SmartMealPlanner.Infrastructure.Identity;
using SmartMealPlanner.Infrastructure.Persistence.Seed;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public async Task<UserHouseholdInfo> ProvisionNewUserHouseholdAsync(
        Guid userId,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = $"{displayName}'s Family",
            CreatedByUserId = userId
        };

        var membership = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            UserId = userId,
            Role = HouseholdRole.Owner,
            JoinedAt = DateTimeOffset.UtcNow
        };

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = displayName,
            HouseholdId = household.Id
        };

        _dbContext.Households.Add(household);
        _dbContext.HouseholdMembers.Add(membership);
        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserHouseholdInfo(
            household.Id,
            household.Name,
            displayName,
            user.Email ?? string.Empty,
            HouseholdRole.Owner);
    }

    public async Task<UserHouseholdInfo?> GetUserHouseholdInfoAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .Include(p => p.Household)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile is null)
        {
            return null;
        }

        var membership = await _dbContext.HouseholdMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.HouseholdId == profile.HouseholdId,
                cancellationToken);

        if (membership is null)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        return new UserHouseholdInfo(
            profile.HouseholdId,
            profile.Household.Name,
            profile.DisplayName,
            user?.Email ?? string.Empty,
            membership.Role);
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default) =>
        _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(
        string token,
        string? replacedByToken,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetRefreshTokenAsync(token, cancellationToken);
        if (refreshToken is null || refreshToken.RevokedAt is not null)
        {
            return;
        }

        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.ReplacedByToken = replacedByToken;
        await UpdateRefreshTokenAsync(refreshToken, cancellationToken);
    }

    public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var utcNow = DateTimeOffset.UtcNow;
        foreach (var token in tokens)
        {
            token.RevokedAt = utcNow;
        }

        if (tokens.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
