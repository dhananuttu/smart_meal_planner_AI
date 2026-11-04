namespace SmartMealPlanner.Application.Auth.DTOs;

public class AuthUserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public Guid HouseholdId { get; set; }

    public string HouseholdName { get; set; } = string.Empty;

    public string HouseholdRole { get; set; } = string.Empty;

    public string PlatformRole { get; set; } = string.Empty;
}
