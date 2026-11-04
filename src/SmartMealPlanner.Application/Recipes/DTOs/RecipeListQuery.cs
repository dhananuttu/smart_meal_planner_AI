namespace SmartMealPlanner.Application.Recipes.DTOs;

public class RecipeListQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }
}
