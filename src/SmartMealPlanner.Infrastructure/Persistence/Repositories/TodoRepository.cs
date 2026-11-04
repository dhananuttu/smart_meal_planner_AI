using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Todos.DTOs;
using SmartMealPlanner.Application.Todos.Helpers;
using SmartMealPlanner.Application.Todos.Interfaces;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TodoRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<TodoDto>> GetPagedAsync(
        Guid userId,
        TodoListQuery query,
        CancellationToken cancellationToken = default)
    {
        var todos = _dbContext.Todos
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        todos = ApplyFilters(todos, query);

        var totalCount = await todos.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var results = await todos
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<TodoDto>.Create(
            results.Select(MapToDto).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<IReadOnlyList<TodoDto>> GetTodayAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Todos
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.DueDate == today)
            .OrderBy(t => t.IsCompleted)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TodoDto>> GetUpcomingAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Todos
            .AsNoTracking()
            .Where(t => t.UserId == userId
                        && !t.IsCompleted
                        && t.DueDate != null
                        && t.DueDate > today)
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TodoDto>> GetOverdueAsync(
        Guid userId,
        DateOnly today,
        CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Todos
            .AsNoTracking()
            .Where(t => t.UserId == userId
                        && !t.IsCompleted
                        && t.DueDate != null
                        && t.DueDate < today)
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TodoDto>> GetRecurringAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Todos
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.IsRecurring)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);

        return results.Select(MapToDto).ToList();
    }

    public async Task<TodoDto?> GetByIdAsync(
        Guid userId,
        Guid todoId,
        CancellationToken cancellationToken = default)
    {
        var todo = await _dbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId, cancellationToken);

        return todo is null ? null : MapToDto(todo);
    }

    public Task<bool> MealPlanEntryExistsAsync(
        Guid mealPlanEntryId,
        CancellationToken cancellationToken = default) =>
        _dbContext.MealPlanEntries.AnyAsync(e => e.Id == mealPlanEntryId, cancellationToken);

    public async Task<TodoDto> CreateAsync(
        Guid userId,
        CreateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Category = TodoEnumHelper.ParseCategory(request.Category),
            Priority = TodoEnumHelper.ParsePriority(request.Priority),
            DueDate = request.DueDate,
            IsRecurring = request.IsRecurring,
            RecurrenceType = request.IsRecurring
                ? TodoEnumHelper.ParseRecurrenceType(request.RecurrenceType)
                : null,
            MealPlanEntryId = request.MealPlanEntryId
        };

        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(todo);
    }

    public async Task<TodoDto?> UpdateAsync(
        Guid userId,
        Guid todoId,
        UpdateTodoRequest request,
        CancellationToken cancellationToken = default)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId, cancellationToken);

        if (todo is null)
        {
            return null;
        }

        todo.Title = request.Title.Trim();
        todo.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        todo.Category = TodoEnumHelper.ParseCategory(request.Category);
        todo.Priority = TodoEnumHelper.ParsePriority(request.Priority);
        todo.DueDate = request.DueDate;
        todo.IsRecurring = request.IsRecurring;
        todo.RecurrenceType = request.IsRecurring
            ? TodoEnumHelper.ParseRecurrenceType(request.RecurrenceType)
            : null;
        todo.MealPlanEntryId = request.MealPlanEntryId;
        ApplyCompletionState(todo, request.IsCompleted, DateTimeOffset.UtcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(todo);
    }

    public async Task<TodoDto?> MarkCompleteAsync(
        Guid userId,
        Guid todoId,
        bool isCompleted,
        DateTimeOffset completedAt,
        CancellationToken cancellationToken = default)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId, cancellationToken);

        if (todo is null)
        {
            return null;
        }

        ApplyCompletionState(todo, isCompleted, completedAt);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(todo);
    }

    public async Task<bool> DeleteAsync(
        Guid userId,
        Guid todoId,
        DateTimeOffset deletedAt,
        CancellationToken cancellationToken = default)
    {
        var todo = await _dbContext.Todos
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId, cancellationToken);

        if (todo is null)
        {
            return false;
        }

        todo.IsDeleted = true;
        todo.DeletedAt = deletedAt;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Todo> ApplyFilters(IQueryable<Todo> todos, TodoListQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = TodoEnumHelper.ParseCategory(query.Category);
            todos = todos.Where(t => t.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            var priority = TodoEnumHelper.ParsePriority(query.Priority);
            todos = todos.Where(t => t.Priority == priority);
        }

        if (query.IsCompleted.HasValue)
        {
            todos = todos.Where(t => t.IsCompleted == query.IsCompleted.Value);
        }

        if (query.IsRecurring.HasValue)
        {
            todos = todos.Where(t => t.IsRecurring == query.IsRecurring.Value);
        }

        return todos;
    }

    private static void ApplyCompletionState(Todo todo, bool isCompleted, DateTimeOffset completedAt)
    {
        todo.IsCompleted = isCompleted;
        todo.CompletedAt = isCompleted ? completedAt : null;
    }

    private static TodoDto MapToDto(Todo todo) =>
        new()
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Category = TodoEnumHelper.ToApiValue(todo.Category),
            Priority = TodoEnumHelper.ToApiValue(todo.Priority),
            DueDate = todo.DueDate,
            IsCompleted = todo.IsCompleted,
            IsRecurring = todo.IsRecurring,
            RecurrenceType = TodoEnumHelper.ToApiValue(todo.RecurrenceType),
            MealPlanEntryId = todo.MealPlanEntryId,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
}
