namespace SmartMealPlanner.Application.Todos.DTOs;

public class UpdateTodoRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Category { get; set; } = "Personal";

    public string Priority { get; set; } = "Medium";

    public DateOnly? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsRecurring { get; set; }

    public string? RecurrenceType { get; set; }

    public Guid? MealPlanEntryId { get; set; }
}
