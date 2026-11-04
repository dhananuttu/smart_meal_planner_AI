namespace SmartMealPlanner.Application.MealPlanner.DTOs;

public class MealRecipeSummaryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
}
