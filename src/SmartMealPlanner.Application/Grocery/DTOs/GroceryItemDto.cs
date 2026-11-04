namespace SmartMealPlanner.Application.Grocery.DTOs;

public class GroceryItemDto
{
    public Guid Id { get; set; }

    public Guid GroceryListId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public bool IsPurchased { get; set; }

    public Guid? SourceRecipeId { get; set; }

    public string? Note { get; set; }

    public int SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
