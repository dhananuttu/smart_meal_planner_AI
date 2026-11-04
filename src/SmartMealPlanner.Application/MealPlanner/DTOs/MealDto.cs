namespace SmartMealPlanner.Application.MealPlanner.DTOs;

public class MealDto
{
    public Guid Id { get; set; }

    public Guid MealPlanId { get; set; }

    public DateOnly Date { get; set; }

    public string MealType { get; set; } = string.Empty;

    public Guid? RecipeId { get; set; }

    public MealRecipeSummaryDto? Recipe { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
