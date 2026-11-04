namespace SmartMealPlanner.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "SmartMealPlanner";

    public string Audience { get; set; } = "SmartMealPlanner";

    public string Secret { get; set; } = string.Empty;

    public int AccessTokenExpirationMinutes { get; set; } = 60;

    public int RefreshTokenExpirationDays { get; set; } = 7;
}
