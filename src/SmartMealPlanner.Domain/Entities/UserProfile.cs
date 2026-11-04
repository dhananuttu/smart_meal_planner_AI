using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class UserProfile : AuditableEntity
{
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public List<string> DietaryPreferences { get; set; } = [];

    public int DefaultServings { get; set; } = 4;

    public string Timezone { get; set; } = "UTC";

    public Guid HouseholdId { get; set; }

    public Household Household { get; set; } = null!;
}
