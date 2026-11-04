using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class PantryCategory : AuditableEntity
{
    public Guid? HouseholdId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsSystem { get; set; }

    public int SortOrder { get; set; }

    public Household? Household { get; set; }
}
