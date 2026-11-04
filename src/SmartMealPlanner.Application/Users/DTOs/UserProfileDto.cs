namespace SmartMealPlanner.Application.Users.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public IReadOnlyList<string> DietaryPreferences { get; set; } = [];

    public int DefaultServings { get; set; }

    public string Timezone { get; set; } = "UTC";

    public Guid HouseholdId { get; set; }

    public string HouseholdName { get; set; } = string.Empty;

    public string HouseholdRole { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
