using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Ingredients.Helpers;

public static class IngredientCategoryHelper
{
    public static IngredientCategory? Parse(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return null;
        }

        return Enum.TryParse<IngredientCategory>(category.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : throw new ArgumentException($"'{category}' is not a valid ingredient category.");
    }

    public static string? ToApiValue(IngredientCategory? category) =>
        category?.ToString().ToLowerInvariant();
}
