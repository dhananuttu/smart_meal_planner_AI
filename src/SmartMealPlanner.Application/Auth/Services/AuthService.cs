using SmartMealPlanner.Application.Auth.DTOs;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Exceptions;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserAccountManager _userAccountManager;
    private readonly IAuthRepository _authRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserAccountManager userAccountManager,
        IAuthRepository authRepository,
        ITokenService tokenService)
    {
        _userAccountManager = userAccountManager;
        _authRepository = authRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        if (await _authRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = await _userAccountManager.CreateUserAsync(request.Email.Trim(), request.Password, cancellationToken);
        var householdInfo = await _authRepository.ProvisionNewUserHouseholdAsync(
            user.Id,
            request.DisplayName.Trim(),
            cancellationToken);

        return await IssueAuthResponseAsync(user, householdInfo, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userAccountManager.ValidateCredentialsAsync(
            request.Email.Trim(),
            request.Password,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException();
        }

        var householdInfo = await _authRepository.GetUserHouseholdInfoAsync(user.Id, cancellationToken);
        if (householdInfo is null)
        {
            throw new UnauthorizedException();
        }

        return await IssueAuthResponseAsync(user, householdInfo, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || !storedToken.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var householdInfo = await _authRepository.GetUserHouseholdInfoAsync(storedToken.UserId, cancellationToken);
        if (householdInfo is null)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var user = new AuthUser(storedToken.UserId, householdInfo.Email);
        var platformRole = await _userAccountManager.GetPlatformRoleAsync(storedToken.UserId, cancellationToken);
        var tokens = _tokenService.GenerateTokens(user, householdInfo, platformRole);

        await _authRepository.RevokeRefreshTokenAsync(storedToken.Token, tokens.RefreshToken, cancellationToken);
        await PersistRefreshTokenAsync(storedToken.UserId, tokens, cancellationToken);

        return BuildAuthResponse(user, householdInfo, tokens, platformRole);
    }

    public async Task LogoutAsync(Guid userId, LogoutRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await _authRepository.RevokeRefreshTokenAsync(request.RefreshToken, replacedByToken: null, cancellationToken);
            return;
        }

        await _authRepository.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
    }

    private async Task<AuthResponse> IssueAuthResponseAsync(
        AuthUser user,
        UserHouseholdInfo householdInfo,
        CancellationToken cancellationToken)
    {
        var platformRole = await _userAccountManager.GetPlatformRoleAsync(user.Id, cancellationToken);
        var tokens = _tokenService.GenerateTokens(user, householdInfo, platformRole);
        await PersistRefreshTokenAsync(user.Id, tokens, cancellationToken);
        return BuildAuthResponse(user, householdInfo, tokens, platformRole);
    }

    private async Task PersistRefreshTokenAsync(Guid userId, TokenPair tokens, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = tokens.RefreshToken,
            ExpiresAt = tokens.RefreshTokenExpiresAt,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _authRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
    }

    private static AuthResponse BuildAuthResponse(
        AuthUser user,
        UserHouseholdInfo householdInfo,
        TokenPair tokens,
        string platformRole) =>
        new()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt,
            User = new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = householdInfo.DisplayName,
                HouseholdId = householdInfo.HouseholdId,
                HouseholdName = householdInfo.HouseholdName,
                HouseholdRole = householdInfo.HouseholdRole == HouseholdRole.Owner ? "owner" : "member",
                PlatformRole = platformRole
            }
        };
}
