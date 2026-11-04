using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Todos.DTOs;
using SmartMealPlanner.Application.Todos.Helpers;
using SmartMealPlanner.Application.Todos.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Todos.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeService _dateTimeService;

    public TodoService(
        ITodoRepository todoRepository,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public Task<PaginatedList<TodoDto>> GetTodosAsync(
        TodoListQuery query,
        CancellationToken cancellationToken = default) =>
        _todoRepository.GetPagedAsync(GetCurrentUserId(), query, cancellationToken);

    public Task<IReadOnlyList<TodoDto>> GetTodayTasksAsync(CancellationToken cancellationToken = default) =>
        _todoRepository.GetTodayAsync(
            GetCurrentUserId(),
            _dateTimeService.TodayUtc,
            cancellationToken);

    public Task<IReadOnlyList<TodoDto>> GetUpcomingTasksAsync(CancellationToken cancellationToken = default) =>
        _todoRepository.GetUpcomingAsync(
            GetCurrentUserId(),
            _dateTimeService.TodayUtc,
            cancellationToken);

    public Task<IReadOnlyList<TodoDto>> GetOverdueTasksAsync(CancellationToken cancellationToken = default) =>
        _todoRepository.GetOverdueAsync(
            GetCurrentUserId(),
            _dateTimeService.TodayUtc,
            cancellationToken);

    public Task<IReadOnlyList<TodoDto>> GetRecurringTasksAsync(CancellationToken cancellationToken = default) =>
        _todoRepository.GetRecurringAsync(GetCurrentUserId(), cancellationToken);

    public async Task<TodoDto> GetByIdAsync(Guid todoId, CancellationToken cancellationToken = default) =>
        await _todoRepository.GetByIdAsync(GetCurrentUserId(), todoId, cancellationToken)
        ?? throw new NotFoundException("Todo", todoId);

    public async Task<TodoDto> CreateAsync(
        CreateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateMealPlanEntryAsync(request.MealPlanEntryId, cancellationToken);
        return await _todoRepository.CreateAsync(GetCurrentUserId(), request, cancellationToken);
    }

    public async Task<TodoDto> UpdateAsync(
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateMealPlanEntryAsync(request.MealPlanEntryId, cancellationToken);

        return await _todoRepository.UpdateAsync(GetCurrentUserId(), todoId, request, cancellationToken)
               ?? throw new NotFoundException("Todo", todoId);
    }

    public async Task<TodoDto> MarkCompleteAsync(
        Guid todoId,
        MarkTodoCompleteRequest request,
        CancellationToken cancellationToken = default) =>
        await _todoRepository.MarkCompleteAsync(
                   GetCurrentUserId(),
                   todoId,
                   request.IsCompleted,
                   _dateTimeService.UtcNow,
                   cancellationToken)
               ?? throw new NotFoundException("Todo", todoId);

    public async Task DeleteAsync(Guid todoId, CancellationToken cancellationToken = default)
    {
        if (!await _todoRepository.DeleteAsync(
                GetCurrentUserId(),
                todoId,
                _dateTimeService.UtcNow,
                cancellationToken))
        {
            throw new NotFoundException("Todo", todoId);
        }
    }

    private async Task ValidateMealPlanEntryAsync(
        Guid? mealPlanEntryId,
        CancellationToken cancellationToken)
    {
        if (!mealPlanEntryId.HasValue)
        {
            return;
        }

        if (!await _todoRepository.MealPlanEntryExistsAsync(mealPlanEntryId.Value, cancellationToken))
        {
            throw new NotFoundException("Meal plan entry", mealPlanEntryId.Value);
        }
    }

    private Guid GetCurrentUserId() =>
        _currentUserService.UserId
        ?? throw new UnauthorizedException("User is not authenticated.");
}
