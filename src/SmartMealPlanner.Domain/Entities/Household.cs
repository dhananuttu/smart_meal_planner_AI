using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class Household : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public ICollection<HouseholdMember> Members { get; set; } = [];

    public ICollection<UserProfile> UserProfiles { get; set; } = [];
}
