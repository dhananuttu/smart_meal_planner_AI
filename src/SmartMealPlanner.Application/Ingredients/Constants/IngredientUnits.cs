namespace SmartMealPlanner.Application.Ingredients.Constants;

public static class IngredientUnits
{
    public static readonly IReadOnlySet<string> AllowedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "each",
        "g",
        "kg",
        "ml",
        "l",
        "cup",
        "tbsp",
        "tsp",
        "oz",
        "lb"
    };
}
