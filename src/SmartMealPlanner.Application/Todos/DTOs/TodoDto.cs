namespace SmartMealPlanner.Application.Todos.DTOs;

public class TodoDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateOnly? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsRecurring { get; set; }

    public string? RecurrenceType { get; set; }

    public Guid? MealPlanEntryId { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
