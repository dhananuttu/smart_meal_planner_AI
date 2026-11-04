using SmartMealPlanner.Application.Auth.Models;

namespace SmartMealPlanner.Application.Auth.Interfaces;

public interface IUserAccountManager
{
    Task<AuthUser> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<string> GetPlatformRoleAsync(Guid userId, CancellationToken cancellationToken = default);
}
