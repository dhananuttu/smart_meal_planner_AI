namespace SmartMealPlanner.Application.Grocery.DTOs;

public class ReceiptGroceryItemRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1;

    public string Unit { get; set; } = string.Empty;

    public string? Note { get; set; }
}
