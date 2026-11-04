using FluentAssertions;
using SmartMealPlanner.Application.MealPlanner.Helpers;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class MealPlannerDateHelperTests
{
    [Fact]
    public void GetWeekStartMonday_ReturnsMonday()
    {
        var wednesday = new DateOnly(2026, 6, 24);
        var weekStart = MealPlannerDateHelper.GetWeekStartMonday(wednesday);

        weekStart.DayOfWeek.Should().Be(DayOfWeek.Monday);
        weekStart.Should().Be(new DateOnly(2026, 6, 22));
    }

    [Fact]
    public void IsDateInWeek_ReturnsTrue_ForDatesInSameWeek()
    {
        var weekStart = new DateOnly(2026, 6, 22);
        MealPlannerDateHelper.IsDateInWeek(new DateOnly(2026, 6, 28), weekStart).Should().BeTrue();
    }
}
