using FluentAssertions;
using SmartMealPlanner.Application.Users.DTOs;
using SmartMealPlanner.Application.Users.Validators;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class UpdateUserProfileRequestValidatorTests
{
    private readonly UpdateUserProfileRequestValidator _validator = new();

    [Fact]
    public void Should_Accept_Valid_Request()
    {
        var request = new UpdateUserProfileRequest
        {
            DisplayName = "Test User",
            DefaultServings = 4,
            Timezone = "UTC",
            DietaryPreferences = ["vegetarian"]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Reject_Invalid_Dietary_Preference()
    {
        var request = new UpdateUserProfileRequest
        {
            DisplayName = "Test User",
            DefaultServings = 4,
            Timezone = "UTC",
            DietaryPreferences = ["paleo"]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Reject_Invalid_Timezone()
    {
        var request = new UpdateUserProfileRequest
        {
            DisplayName = "Test User",
            DefaultServings = 4,
            Timezone = "Not/A/Timezone",
            DietaryPreferences = []
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}
