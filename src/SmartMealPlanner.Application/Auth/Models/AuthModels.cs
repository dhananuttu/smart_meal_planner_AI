using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Auth.Models;

public record AuthUser(Guid Id, string Email);

public record UserHouseholdInfo(
    Guid HouseholdId,
    string HouseholdName,
    string DisplayName,
    string Email,
    HouseholdRole HouseholdRole);

public record TokenPair(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);
