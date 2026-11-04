namespace SmartMealPlanner.Domain.Common;

public abstract class UserOwnedEntity : AuditableEntity
{
    public Guid UserId { get; set; }
}
