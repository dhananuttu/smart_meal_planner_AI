using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Application.Ingredients.Interfaces;
using SmartMealPlanner.Application.Ingredients.Services;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class IngredientServiceTests
{
    private readonly Mock<IIngredientRepository> _repository = new();
    private readonly Mock<IHouseholdContext> _householdContext = new();
    private readonly IngredientService _sut;
    private readonly Guid _householdId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public IngredientServiceTests()
    {
        _householdContext.Setup(x => x.HouseholdId).Returns(_householdId);
        _householdContext.Setup(x => x.UserId).Returns(_userId);
        _householdContext.Setup(x => x.EnsureMemberAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new IngredientService(_repository.Object, _householdContext.Object);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsConflict()
    {
        _repository
            .Setup(x => x.NameExistsAsync(_householdId, "spinach", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _sut.CreateAsync(new CreateIngredientRequest
        {
            Name = "Spinach",
            Category = "produce",
            DefaultUnit = "g"
        });

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsIngredient()
    {
        var ingredientId = Guid.NewGuid();
        _repository
            .Setup(x => x.NameExistsAsync(_householdId, "spinach", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository
            .Setup(x => x.CreateAsync(
                _householdId,
                _userId,
                "Spinach",
                "g",
                IngredientCategory.Produce,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IngredientDto
            {
                Id = ingredientId,
                Name = "Spinach",
                Category = "produce",
                DefaultUnit = "g",
                HouseholdId = _householdId
            });

        var result = await _sut.CreateAsync(new CreateIngredientRequest
        {
            Name = "Spinach",
            Category = "produce",
            DefaultUnit = "g"
        });

        result.Name.Should().Be("Spinach");
        result.Category.Should().Be("produce");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ThrowsNotFound()
    {
        var ingredientId = Guid.NewGuid();
        _repository
            .Setup(x => x.GetByIdAsync(_householdId, ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientDto?)null);

        var act = () => _sut.GetByIdAsync(ingredientId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
