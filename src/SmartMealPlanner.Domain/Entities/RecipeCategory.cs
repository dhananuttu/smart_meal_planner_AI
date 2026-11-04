using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class RecipeCategory : HouseholdEntity
{
    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#000000";

    public Household Household { get; set; } = null!;

    public ICollection<Recipe> Recipes { get; set; } = [];
}
