using Microsoft.AspNetCore.Identity;
using SmartMealPlanner.Application.Auth.Interfaces;
using SmartMealPlanner.Application.Auth.Models;
using SmartMealPlanner.Domain.Exceptions;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Identity;

public class UserAccountManager : IUserAccountManager
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserAccountManager(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthUser> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("A user with this email already exists.");
            }

            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ValidationException(errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
            throw new ValidationException(errors);
        }

        return new AuthUser(user.Id, user.Email ?? email);
    }

    public async Task<AuthUser?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var valid = await _userManager.CheckPasswordAsync(user, password);
        return valid ? new AuthUser(user.Id, user.Email ?? email) : null;
    }

    public async Task<string> GetPlatformRoleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return "User";
        }

        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault() ?? "User";
    }
}
