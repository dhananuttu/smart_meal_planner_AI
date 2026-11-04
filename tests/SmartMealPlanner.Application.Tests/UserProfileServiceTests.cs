using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Users.DTOs;
using SmartMealPlanner.Application.Users.Interfaces;
using SmartMealPlanner.Application.Users.Models;
using SmartMealPlanner.Application.Users.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repository = new();
    private readonly UserProfileService _sut;

    public UserProfileServiceTests()
    {
        _sut = new UserProfileService(_repository.Object);
    }

    [Fact]
    public async Task GetMyProfileAsync_WhenProfileMissing_ThrowsNotFound()
    {
        var userId = Guid.NewGuid();
        _repository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfileDetail?)null);

        var act = () => _sut.GetMyProfileAsync(userId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetMyProfileAsync_WhenProfileExists_ReturnsDto()
    {
        var userId = Guid.NewGuid();
        var detail = CreateDetail(userId);

        _repository
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detail);

        var result = await _sut.GetMyProfileAsync(userId);

        result.Email.Should().Be("user@test.local");
        result.HouseholdName.Should().Be("Test Family");
        result.HouseholdRole.Should().Be("owner");
    }

    [Fact]
    public async Task UpdateMyProfileAsync_NormalizesDietaryPreferences()
    {
        var userId = Guid.NewGuid();
        var detail = CreateDetail(userId);
        detail.DietaryPreferences = ["vegetarian", "gluten-free"];

        _repository
            .Setup(x => x.UpdateAsync(
                userId,
                "Updated Name",
                null,
                It.Is<IReadOnlyList<string>>(p => p.SequenceEqual(new[] { "vegetarian", "gluten-free" })),
                6,
                "UTC",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(detail);

        var result = await _sut.UpdateMyProfileAsync(userId, new UpdateUserProfileRequest
        {
            DisplayName = "Updated Name",
            DietaryPreferences = [" Vegetarian ", "gluten-free", "vegetarian"],
            DefaultServings = 6,
            Timezone = "UTC"
        });

        result.DietaryPreferences.Should().BeEquivalentTo(["vegetarian", "gluten-free"]);
    }

    private static UserProfileDetail CreateDetail(Guid userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = "user@test.local",
            DisplayName = "Test User",
            DefaultServings = 4,
            Timezone = "UTC",
            HouseholdId = Guid.NewGuid(),
            HouseholdName = "Test Family",
            HouseholdRole = "owner",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
}
