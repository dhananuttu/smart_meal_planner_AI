using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Infrastructure.Auth;

namespace SmartMealPlanner.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenPair GenerateTokens(AuthUser user, UserHouseholdInfo householdInfo, string platformRole)
    {
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);

        var accessToken = GenerateAccessToken(
            user,
            householdInfo,
            platformRole,
            accessTokenExpiresAt);

        var refreshToken = GenerateRefreshToken();

        return new TokenPair(
            accessToken,
            refreshToken,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }

    private string GenerateAccessToken(
        AuthUser user,
        UserHouseholdInfo householdInfo,
        string platformRole,
        DateTimeOffset expiresAt)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, platformRole),
            new("household_id", householdInfo.HouseholdId.ToString()),
            new("household_role", householdInfo.HouseholdRole.ToString().ToLowerInvariant())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
