using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Todos.DTOs;

namespace SmartMealPlanner.Application.Todos.Interfaces;

public interface ITodoService
{
    Task<PaginatedList<TodoDto>> GetTodosAsync(
        TodoListQuery query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetTodayTasksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetUpcomingTasksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TodoDto>> GetRecurringTasksAsync(CancellationToken cancellationToken = default);

    Task<TodoDto> GetByIdAsync(Guid todoId, CancellationToken cancellationToken = default);

    Task<TodoDto> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken = default);

    Task<TodoDto> UpdateAsync(Guid todoId, UpdateTodoRequest request, CancellationToken cancellationToken = default);

    Task<TodoDto> MarkCompleteAsync(
        Guid todoId,
        MarkTodoCompleteRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid todoId, CancellationToken cancellationToken = default);
}
