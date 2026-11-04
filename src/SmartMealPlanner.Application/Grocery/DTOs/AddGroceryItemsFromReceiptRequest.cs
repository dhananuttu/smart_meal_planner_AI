namespace SmartMealPlanner.Application.Grocery.DTOs;

public class AddGroceryItemsFromReceiptRequest
{
    public List<ReceiptGroceryItemRequest> Items { get; set; } = [];
}
