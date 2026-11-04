using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class PantryItem : HouseholdEntity
{
    public Guid IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal? LowStockThreshold { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public Household Household { get; set; } = null!;

    public Ingredient Ingredient { get; set; } = null!;
}
