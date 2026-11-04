using SmartMealPlanner.Application.Grocery.Models;

namespace SmartMealPlanner.Application.Grocery.Helpers;

public static class GroceryIngredientAggregator
{
    public static IReadOnlyList<AggregatedGroceryIngredient> Aggregate(
        IEnumerable<RecipeIngredientLine> ingredients)
    {
        return ingredients
            .GroupBy(i => (Name: i.Name.Trim().ToLowerInvariant(), Unit: i.Unit.Trim().ToLowerInvariant()))
            .Select(group =>
            {
                var first = group.First();
                return new AggregatedGroceryIngredient
                {
                    Name = first.Name.Trim(),
                    Unit = first.Unit.Trim().ToLowerInvariant(),
                    Quantity = group.Sum(i => i.Quantity),
                    SourceRecipeId = group.Select(i => i.RecipeId).FirstOrDefault()
                };
            })
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<AggregatedGroceryIngredient> MergeItems(
        IEnumerable<AggregatedGroceryIngredient> existing,
        IEnumerable<AggregatedGroceryIngredient> incoming)
    {
        var combined = existing
            .Select(i => new RecipeIngredientLine
            {
                Name = i.Name,
                Unit = i.Unit,
                Quantity = i.Quantity,
                RecipeId = i.SourceRecipeId ?? Guid.Empty
            })
            .Concat(incoming.Select(i => new RecipeIngredientLine
            {
                Name = i.Name,
                Unit = i.Unit,
                Quantity = i.Quantity,
                RecipeId = i.SourceRecipeId ?? Guid.Empty
            }));

        return Aggregate(combined);
    }
}
