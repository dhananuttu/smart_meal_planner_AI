using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Infrastructure.Services;

public class HouseholdContext : IHouseholdContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public HouseholdContext(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }

    public Guid UserId =>
        _currentUserService.UserId
        ?? throw new ForbiddenException("User is not authenticated.");

    public Guid HouseholdId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("household_id");
            if (Guid.TryParse(claim, out var householdId))
            {
                return householdId;
            }

            throw new ForbiddenException("Household context is not available.");
        }
    }

    public HouseholdRole Role
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("household_role");
            return claim?.ToLowerInvariant() switch
            {
                "owner" => HouseholdRole.Owner,
                "member" => HouseholdRole.Member,
                _ => throw new ForbiddenException("Household role is not available.")
            };
        }
    }

    public bool IsOwner => Role == HouseholdRole.Owner;

    public Task EnsureMemberAsync(CancellationToken cancellationToken = default)
    {
        _ = UserId;
        _ = HouseholdId;
        return Task.CompletedTask;
    }

    public Task EnsureOwnerAsync(CancellationToken cancellationToken = default)
    {
        if (!IsOwner)
        {
            throw new ForbiddenException("Owner role is required for this operation.");
        }

        return Task.CompletedTask;
    }
}
