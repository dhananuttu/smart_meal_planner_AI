using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.MealPlanner.DTOs;
using SmartMealPlanner.Application.MealPlanner.Interfaces;
using SmartMealPlanner.Application.MealPlanner.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class MealPlanServiceTests
{
    private readonly Mock<IMealPlanRepository> _repository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly MealPlanService _sut;
    private readonly Guid _householdId = Guid.NewGuid();

    public MealPlanServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new MealPlanService(_repository.Object, _householdContext.Object);
    }

    [Fact]
    public async Task CreateMealAsync_WithInvalidRecipe_ThrowsNotFound()
    {
        var recipeId = Guid.NewGuid();
        _repository
            .Setup(x => x.RecipeExistsInHouseholdAsync(_householdId, recipeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.CreateMealAsync(new CreateMealRequest
        {
            Date = new DateOnly(2026, 6, 24),
            MealType = "Breakfast",
            RecipeId = recipeId
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetWeeklyPlanAsync_NormalizesWeekStartToMonday()
    {
        var weekStart = new DateOnly(2026, 6, 24);
        _repository
            .Setup(x => x.GetOrCreateWeeklyPlanAsync(
                _householdId,
                It.IsAny<Guid>(),
                new DateOnly(2026, 6, 22),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WeeklyMealPlanDto
            {
                PlanId = Guid.NewGuid(),
                WeekStartDate = new DateOnly(2026, 6, 22),
                WeekEndDate = new DateOnly(2026, 6, 28)
            });

        var result = await _sut.GetWeeklyPlanAsync(weekStart);

        result.WeekStartDate.Should().Be(new DateOnly(2026, 6, 22));
    }
}
