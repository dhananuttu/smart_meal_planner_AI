using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Application.Auth.Interfaces;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<UserHouseholdInfo> ProvisionNewUserHouseholdAsync(
        Guid userId,
        string displayName,
        CancellationToken cancellationToken = default);

    Task<UserHouseholdInfo?> GetUserHouseholdInfoAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(string token, string? replacedByToken, CancellationToken cancellationToken = default);

    Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
