using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class InventoryAlert : AuditableEntity
{
    public Guid HouseholdId { get; set; }

    public Guid PantryItemId { get; set; }

    public AlertType AlertType { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = "medium";

    public string ConditionSnapshot { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Household Household { get; set; } = null!;

    public PantryItem PantryItem { get; set; } = null!;
}
