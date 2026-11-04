using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class Ingredient : HouseholdEntity
{
    public string Name { get; set; } = string.Empty;

    public string? DefaultUnit { get; set; }

    public IngredientCategory? Category { get; set; }

    public string? Notes { get; set; }

    public Household Household { get; set; } = null!;
}
