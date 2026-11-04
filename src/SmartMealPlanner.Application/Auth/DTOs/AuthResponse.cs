namespace SmartMealPlanner.Application.Auth.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public AuthUserDto User { get; set; } = new();
}
