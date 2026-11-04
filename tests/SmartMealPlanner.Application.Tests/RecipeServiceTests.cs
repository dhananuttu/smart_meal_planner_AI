using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Application.Recipes.DTOs;
using SmartMealPlanner.Application.Recipes.Interfaces;
using SmartMealPlanner.Application.Recipes.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _recipeRepository = new();
    private readonly Mock<IRecipeCategoryRepository> _categoryRepository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly RecipeService _sut;
    private readonly Guid _householdId = Guid.NewGuid();

    public RecipeServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new RecipeService(
            _recipeRepository.Object,
            _categoryRepository.Object,
            _householdContext.Object);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategory_ThrowsNotFound()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository
            .Setup(x => x.ExistsInHouseholdAsync(_householdId, categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.CreateAsync(new CreateRecipeRequest
        {
            Name = "Green Smoothie",
            Instructions = "Blend all ingredients until smooth.",
            Servings = 2,
            CategoryId = categoryId
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ThrowsNotFound()
    {
        var recipeId = Guid.NewGuid();
        _recipeRepository
            .Setup(x => x.GetByIdAsync(_householdId, recipeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecipeDto?)null);

        var act = () => _sut.GetByIdAsync(recipeId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
