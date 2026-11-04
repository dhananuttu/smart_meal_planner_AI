namespace SmartMealPlanner.Application.InventoryMonitoring.Models;

public class PantryItemMonitoringSnapshot
{
    public Guid PantryItemId { get; init; }

    public Guid HouseholdId { get; init; }

    public string IngredientName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public string Unit { get; init; } = string.Empty;

    public decimal? LowStockThreshold { get; init; }

    public DateOnly? ExpiryDate { get; init; }
}
