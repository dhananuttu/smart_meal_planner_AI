namespace SmartMealPlanner.Application.Pantry.DTOs;

public class PantryIngredientSummaryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Category { get; set; }
}
