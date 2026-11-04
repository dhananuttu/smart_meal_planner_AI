namespace SmartMealPlanner.Application.Pantry.DTOs;

public class UpdatePantryItemRequest
{
    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }

    public decimal? LowStockThreshold { get; set; }
}
