namespace SmartMealPlanner.Application.Ingredients.DTOs;

public class IngredientDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? DefaultUnit { get; set; }

    public string? Category { get; set; }

    public string? Notes { get; set; }

    public Guid HouseholdId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
