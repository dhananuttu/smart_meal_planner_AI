using FluentAssertions;
using SmartMealPlanner.Application.RecipeCategories.DTOs;
using SmartMealPlanner.Application.RecipeCategories.Validators;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class RecipeCategoryValidatorTests
{
    [Fact]
    public void Should_Accept_Valid_Color()
    {
        var validator = new CreateRecipeCategoryRequestValidator();
        var result = validator.Validate(new CreateRecipeCategoryRequest
        {
            Name = "Breakfast",
            Color = "#4CAF50"
        });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Reject_Invalid_Color()
    {
        var validator = new CreateRecipeCategoryRequestValidator();
        var result = validator.Validate(new CreateRecipeCategoryRequest
        {
            Name = "Breakfast",
            Color = "green"
        });

        result.IsValid.Should().BeFalse();
    }
}
