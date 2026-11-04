using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Auth.DTOs;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Application.Auth.Services;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserAccountManager> _userAccountManager = new();
    private readonly Mock<IAuthRepository> _authRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _userAccountManager.Object,
            _authRepository.Object,
            _tokenService.Object);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ThrowsUnauthorized()
    {
        _userAccountManager
            .Setup(x => x.ValidateCredentialsAsync("bad@test.local", "wrong", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthUser?)null);

        var act = () => _sut.LoginAsync(new LoginRequest
        {
            Email = "bad@test.local",
            Password = "wrong"
        });

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsConflict()
    {
        _authRepository
            .Setup(x => x.EmailExistsAsync("EXISTING@TEST.LOCAL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _sut.RegisterAsync(new RegisterRequest
        {
            Email = "existing@test.local",
            Password = "Password1",
            DisplayName = "Test"
        });

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokens()
    {
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();
        var user = new AuthUser(userId, "user@test.local");
        var household = new UserHouseholdInfo(householdId, "Test Family", "Test User", "user@test.local", HouseholdRole.Owner);
        var tokens = new TokenPair("access", "refresh", DateTimeOffset.UtcNow.AddMinutes(60), DateTimeOffset.UtcNow.AddDays(7));

        _userAccountManager
            .Setup(x => x.ValidateCredentialsAsync("user@test.local", "Password1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authRepository
            .Setup(x => x.GetUserHouseholdInfoAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(household);
        _userAccountManager
            .Setup(x => x.GetPlatformRoleAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("User");
        _tokenService
            .Setup(x => x.GenerateTokens(user, household, "User"))
            .Returns(tokens);
        _authRepository
            .Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.LoginAsync(new LoginRequest
        {
            Email = "user@test.local",
            Password = "Password1"
        });

        result.AccessToken.Should().Be("access");
        result.RefreshToken.Should().Be("refresh");
        result.User.HouseholdRole.Should().Be("owner");
    }
}
