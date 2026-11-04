namespace SmartMealPlanner.Application.MealPlanner.DTOs;

public class CreateMealRequest
{
    public DateOnly Date { get; set; }

    public string MealType { get; set; } = string.Empty;

    public Guid? RecipeId { get; set; }

    public string? Notes { get; set; }
}
