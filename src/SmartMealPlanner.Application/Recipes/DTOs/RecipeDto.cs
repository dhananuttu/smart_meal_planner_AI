namespace SmartMealPlanner.Application.Recipes.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public int? PrepTime { get; set; }

    public int? CookTime { get; set; }

    public int Servings { get; set; }

    public Guid? CategoryId { get; set; }

    public RecipeCategorySummaryDto? Category { get; set; }

    public Guid HouseholdId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
