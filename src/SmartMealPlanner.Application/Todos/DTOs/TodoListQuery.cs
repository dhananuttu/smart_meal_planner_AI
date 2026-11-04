namespace SmartMealPlanner.Application.Todos.DTOs;

public class TodoListQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Category { get; set; }

    public string? Priority { get; set; }

    public bool? IsCompleted { get; set; }

    public bool? IsRecurring { get; set; }
}
