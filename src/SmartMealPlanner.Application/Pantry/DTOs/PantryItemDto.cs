namespace SmartMealPlanner.Application.Pantry.DTOs;

public class PantryItemDto
{
    public Guid Id { get; set; }

    public Guid IngredientId { get; set; }

    public PantryIngredientSummaryDto Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public DateOnly? ExpirationDate { get; set; }

    public decimal? LowStockThreshold { get; set; }

    public bool IsLowStock { get; set; }

    public bool IsExpired { get; set; }

    public bool IsExpiringSoon { get; set; }

    public Guid HouseholdId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
