namespace SmartMealPlanner.Domain.Common;

public abstract class HouseholdEntity : AuditableEntity
{
    public Guid HouseholdId { get; set; }

    public Guid CreatedByUserId { get; set; }
}
