namespace SmartMealPlanner.Application.InventoryAlerts.DTOs;

public class InventoryAlertPantryItemDto
{
    public Guid Id { get; set; }

    public Guid IngredientId { get; set; }

    public string IngredientName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }
}
