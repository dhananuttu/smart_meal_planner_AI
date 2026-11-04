using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.RecipeCategories.DTOs;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Application.RecipeCategories.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class RecipeCategoryServiceTests
{
    private readonly Mock<IRecipeCategoryRepository> _repository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly RecipeCategoryService _sut;
    private readonly Guid _householdId = Guid.NewGuid();

    public RecipeCategoryServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new RecipeCategoryService(_repository.Object, _householdContext.Object);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsConflict()
    {
        _repository
            .Setup(x => x.NameExistsAsync(_householdId, "breakfast", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _sut.CreateAsync(new CreateRecipeCategoryRequest
        {
            Name = "Breakfast",
            Color = "#4CAF50"
        });

        await act.Should().ThrowAsync<ConflictException>();
    }
}
