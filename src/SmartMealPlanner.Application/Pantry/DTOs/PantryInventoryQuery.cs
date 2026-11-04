namespace SmartMealPlanner.Application.Pantry.DTOs;

public class PantryInventoryQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }

    public bool? LowStockOnly { get; set; }

    public bool? ExpiringSoonOnly { get; set; }
}
