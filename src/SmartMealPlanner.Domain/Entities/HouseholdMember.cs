using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class HouseholdMember : AuditableEntity
{
    public Guid HouseholdId { get; set; }

    public Guid UserId { get; set; }

    public HouseholdRole Role { get; set; } = HouseholdRole.Member;

    public string? DisplayLabel { get; set; }

    public DateTimeOffset JoinedAt { get; set; }

    public Household Household { get; set; } = null!;
}
