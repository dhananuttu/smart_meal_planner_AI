namespace SmartMealPlanner.Application.Grocery.DTOs;

public class GroceryListSummaryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? MealPlanId { get; set; }

    public bool IsArchived { get; set; }

    public int ItemCount { get; set; }

    public int PurchasedItemCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
