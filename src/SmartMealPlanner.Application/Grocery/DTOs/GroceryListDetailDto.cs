namespace SmartMealPlanner.Application.Grocery.DTOs;

public class GroceryListDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? MealPlanId { get; set; }

    public bool IsArchived { get; set; }

    public IReadOnlyList<GroceryItemDto> Items { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
