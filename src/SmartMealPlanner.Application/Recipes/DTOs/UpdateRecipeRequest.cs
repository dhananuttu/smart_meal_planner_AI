namespace SmartMealPlanner.Application.Recipes.DTOs;

public class UpdateRecipeRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public int? PrepTime { get; set; }

    public int? CookTime { get; set; }

    public int Servings { get; set; } = 4;

    public Guid? CategoryId { get; set; }
}
