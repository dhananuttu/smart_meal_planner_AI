using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Todos.DTOs;

namespace SmartMealPlanner.Application.Todos.Interfaces;

public interface ITodoRepository
{
    Task<PaginatedList<TodoDto>> GetPagedAsync(
        Guid userId,
        TodoListQuery query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetTodayAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetUpcomingAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetOverdueAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetRecurringAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<TodoDto?> GetByIdAsync(
        Guid userId,
        Guid todoId,
        CancellationToken cancellationToken = default);

    Task<bool> MealPlanEntryExistsAsync(
        Guid mealPlanEntryId,
        CancellationToken cancellationToken = default);

    Task<TodoDto> CreateAsync(
        Guid userId,
        CreateTodoRequest request,
        CancellationToken cancellationToken = default);

    Task<TodoDto?> UpdateAsync(
        Guid userId,
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default);

    Task<TodoDto?> MarkCompleteAsync(
        Guid userId,
        Guid todoId,
        bool isCompleted,
        DateTimeOffset completedAt,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid userId,
        Guid todoId,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken = default);
}
