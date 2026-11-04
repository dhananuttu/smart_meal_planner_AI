namespace SmartMealPlanner.Application.Pantry.DTOs;

public class CreatePantryItemRequest
{
    public Guid IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }

    public decimal? LowStockThreshold { get; set; }
}
