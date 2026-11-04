namespace SmartMealPlanner.Application.Recipes.DTOs;

public class RecipeCategorySummaryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}
