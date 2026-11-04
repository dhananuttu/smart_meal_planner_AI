using SmartMealPlanner.Application.Auth.DTOs;

namespace SmartMealPlanner.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    Task LogoutAsync(Guid userId, LogoutRequest request, CancellationToken cancellationToken = default);
}
