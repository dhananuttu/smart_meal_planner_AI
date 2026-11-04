using FluentAssertions;
using SmartMealPlanner.Application.Todos.Helpers;
using SmartMealPlanner.Domain.Enums;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class TodoEnumHelperTests
{
    [Theory]
    [InlineData("Meal Preparation", TodoCategory.MealPreparation)]
    [InlineData("Shopping", TodoCategory.Shopping)]
    [InlineData("household", TodoCategory.Household)]
    [InlineData("Personal", TodoCategory.Personal)]
    public void ParseCategory_AcceptsApiValues(string input, TodoCategory expected)
    {
        TodoEnumHelper.ParseCategory(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("Low", TodoPriority.Low)]
    [InlineData("medium", TodoPriority.Medium)]
    [InlineData("High", TodoPriority.High)]
    [InlineData("Critical", TodoPriority.Critical)]
    public void ParsePriority_AcceptsApiValues(string input, TodoPriority expected)
    {
        TodoEnumHelper.ParsePriority(input).Should().Be(expected);
    }

    [Fact]
    public void ToApiValue_MealPreparation_ReturnsSpacedLabel()
    {
        TodoEnumHelper.ToApiValue(TodoCategory.MealPreparation).Should().Be("Meal Preparation");
    }
}
