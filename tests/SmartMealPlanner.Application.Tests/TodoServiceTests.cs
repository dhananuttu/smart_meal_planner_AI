using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Todos.DTOs;
using SmartMealPlanner.Application.Todos.Interfaces;
using SmartMealPlanner.Application.Todos.Services;
using SmartMealPlanner.Domain.Exceptions;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _repository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IDateTimeService> _dateTimeService = new();
    private readonly TodoService _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public TodoServiceTests()
    {
        _currentUserService.Setup(x => x.UserId).Returns(_userId);
        _dateTimeService.Setup(x => x.TodayUtc).Returns(new DateOnly(2026, 6, 22));
        _dateTimeService.Setup(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);

        _sut = new TodoService(
            _repository.Object,
            _currentUserService.Object,
            _dateTimeService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ThrowsNotFound()
    {
        var todoId = Guid.NewGuid();
        _repository
            .Setup(x => x.GetByIdAsync(_userId, todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoDto?)null);

        var act = () => _sut.GetByIdAsync(todoId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithUnknownMealPlanEntry_ThrowsNotFound()
    {
        var mealPlanEntryId = Guid.NewGuid();
        _repository
            .Setup(x => x.MealPlanEntryExistsAsync(mealPlanEntryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.CreateAsync(new CreateTodoRequest
        {
            Title = "Prep chicken",
            Category = "Meal Preparation",
            Priority = "High",
            MealPlanEntryId = mealPlanEntryId
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetTodayTasksAsync_UsesTodayUtcFromDateTimeService()
    {
        _repository
            .Setup(x => x.GetTodayAsync(_userId, new DateOnly(2026, 6, 22), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _sut.GetTodayTasksAsync();

        _repository.Verify(x => x.GetTodayAsync(_userId, new DateOnly(2026, 6, 22), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ThrowsNotFound()
    {
        var todoId = Guid.NewGuid();
        _repository
            .Setup(x => x.DeleteAsync(_userId, todoId, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _sut.DeleteAsync(todoId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
