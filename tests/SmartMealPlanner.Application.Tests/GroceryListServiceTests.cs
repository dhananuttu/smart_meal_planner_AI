using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Grocery.Interfaces;
using SmartMealPlanner.Application.Grocery.Services;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class GroceryListServiceTests
{
    private readonly Mock<IGroceryListRepository> _repository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly GroceryListService _sut;
    private readonly Guid _householdId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public GroceryListServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(_userId);
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new GroceryListService(_repository.Object, _householdContext.Object);
    }

    [Fact]
    public async Task GetListByIdAsync_WhenMissing_ThrowsNotFound()
    {
        var listId = Guid.NewGuid();
        _repository
            .Setup(x => x.GetListByIdAsync(_householdId, listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GroceryListDetailDto?)null);

        var act = () => _sut.GetListByIdAsync(listId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GenerateFromMealPlanAsync_WhenMealPlanMissing_ThrowsNotFound()
    {
        _repository
            .Setup(x => x.ResolveMealPlanAsync(
                _householdId,
                It.IsAny<Guid?>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MealPlan?)null);

        var act = () => _sut.GenerateFromMealPlanAsync(new GenerateGroceryListFromMealPlanRequest
        {
            WeekStart = new DateOnly(2026, 6, 16)
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AddItemAsync_WithUnknownRecipe_ThrowsNotFound()
    {
        var listId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        _repository
            .Setup(x => x.RecipeExistsInHouseholdAsync(_householdId, recipeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.AddItemAsync(listId, new AddGroceryItemRequest
        {
            Name = "Milk",
            Quantity = 1,
            Unit = "l",
            SourceRecipeId = recipeId
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteItemAsync_WhenMissing_ThrowsNotFound()
    {
        var itemId = Guid.NewGuid();
        _repository
            .Setup(x => x.DeleteItemAsync(_householdId, itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.DeleteItemAsync(itemId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
