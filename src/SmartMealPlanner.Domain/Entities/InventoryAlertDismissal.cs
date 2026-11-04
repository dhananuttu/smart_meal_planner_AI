using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class InventoryAlertDismissal
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PantryItemId { get; set; }

    public AlertType AlertType { get; set; }

    public DateTimeOffset DismissedAt { get; set; }

    public string? ConditionSnapshot { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public PantryItem PantryItem { get; set; } = null!;
}
