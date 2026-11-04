namespace SmartMealPlanner.Application.RecipeCategories.DTOs;

public class RecipeCategoryListQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }
}
