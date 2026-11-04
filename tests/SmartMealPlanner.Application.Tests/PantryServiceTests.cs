using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Pantry.DTOs;
using SmartMealPlanner.Application.Pantry.Interfaces;
using SmartMealPlanner.Application.Pantry.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class PantryServiceTests
{
    private readonly Mock<IPantryRepository> _repository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly PantryService _sut;
    private readonly Guid _householdId = Guid.NewGuid();

    public PantryServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new PantryService(_repository.Object, _householdContext.Object);
    }

    [Fact]
    public async Task AddItemAsync_WithDuplicateIngredient_ThrowsConflict()
    {
        var ingredientId = Guid.NewGuid();
        _repository
            .Setup(x => x.IngredientExistsInHouseholdAsync(_householdId, ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repository
            .Setup(x => x.IngredientAlreadyInPantryAsync(_householdId, ingredientId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _sut.AddItemAsync(new CreatePantryItemRequest
        {
            IngredientId = ingredientId,
            Quantity = 2,
            Unit = "kg"
        });

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task AddItemAsync_WithUnknownIngredient_ThrowsNotFound()
    {
        var ingredientId = Guid.NewGuid();
        _repository
            .Setup(x => x.IngredientExistsInHouseholdAsync(_householdId, ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.AddItemAsync(new CreatePantryItemRequest
        {
            IngredientId = ingredientId,
            Quantity = 2,
            Unit = "kg"
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
