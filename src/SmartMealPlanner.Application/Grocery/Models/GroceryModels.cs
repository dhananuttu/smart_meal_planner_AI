namespace SmartMealPlanner.Application.Grocery.Models;

public class AggregatedGroceryIngredient
{
    public string Name { get; init; } = string.Empty;

    public string Unit { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public Guid? SourceRecipeId { get; init; }
}

public class RecipeIngredientLine
{
    public string Name { get; init; } = string.Empty;

    public string Unit { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public Guid RecipeId { get; init; }
}
