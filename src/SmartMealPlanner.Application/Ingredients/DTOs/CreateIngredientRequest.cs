namespace SmartMealPlanner.Application.Ingredients.DTOs;

public class CreateIngredientRequest
{
    public string Name { get; set; } = string.Empty;

    public string? DefaultUnit { get; set; }

    public string? Category { get; set; }

    public string? Notes { get; set; }
}
