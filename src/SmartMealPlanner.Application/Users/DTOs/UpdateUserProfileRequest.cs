namespace SmartMealPlanner.Application.Users.DTOs;

public class UpdateUserProfileRequest
{
    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public List<string> DietaryPreferences { get; set; } = [];

    public int DefaultServings { get; set; } = 4;

    public string Timezone { get; set; } = "UTC";
}
