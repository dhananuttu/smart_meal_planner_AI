using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.InventoryAlerts.DTOs;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryAlerts.Interfaces;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class InventoryAlertRepository : IInventoryAlertRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryAlertRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InventoryAlertDto>> GetAlertsAsync(
        Guid householdId,
        Guid userId,
        InventoryAlertListQuery query,
        CancellationToken cancellationToken = default)
    {
        var alerts = await _dbContext.InventoryAlerts
            .AsNoTracking()
            .Include(a => a.PantryItem)
            .ThenInclude(p => p.Ingredient)
            .Where(a => a.HouseholdId == householdId && a.IsActive)
            .OrderByDescending(a => a.Severity)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var alertIds = alerts.Select(a => a.Id).ToList();
        var reads = await _dbContext.InventoryAlertReads
            .AsNoTracking()
            .Where(r => r.UserId == userId && alertIds.Contains(r.AlertId))
            .ToDictionaryAsync(r => r.AlertId, cancellationToken);

        var dismissals = await _dbContext.InventoryAlertDismissals
            .AsNoTracking()
            .Where(d => d.UserId == userId)
            .ToListAsync(cancellationToken);

        var dismissalLookup = dismissals.ToDictionary(
            d => (d.PantryItemId, d.AlertType),
            d => d);

        var results = new List<InventoryAlertDto>();
        foreach (var alert in alerts)
        {
            var isDismissed = dismissalLookup.TryGetValue((alert.PantryItemId, alert.AlertType), out var dismissal)
                && InventoryAlertEvaluator.MatchesCondition(dismissal.ConditionSnapshot ?? string.Empty, alert.ConditionSnapshot);

            if (!query.IncludeDismissed && isDismissed)
            {
                continue;
            }

            var isRead = reads.ContainsKey(alert.Id);
            if (!query.IncludeRead && isRead)
            {
                continue;
            }

            results.Add(MapToDto(alert, isRead, isDismissed));
        }

        return results;
    }

    public async Task<InventoryAlertDto?> GetByIdAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.InventoryAlerts
            .AsNoTracking()
            .Include(a => a.PantryItem)
            .ThenInclude(p => p.Ingredient)
            .FirstOrDefaultAsync(a => a.Id == alertId && a.HouseholdId == householdId && a.IsActive, cancellationToken);

        if (alert is null)
        {
            return null;
        }

        var isRead = await _dbContext.InventoryAlertReads
            .AnyAsync(r => r.UserId == userId && r.AlertId == alertId, cancellationToken);

        var dismissal = await _dbContext.InventoryAlertDismissals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.UserId == userId && d.PantryItemId == alert.PantryItemId && d.AlertType == alert.AlertType,
                cancellationToken);

        var isDismissed = dismissal is not null
            && InventoryAlertEvaluator.MatchesCondition(dismissal.ConditionSnapshot ?? string.Empty, alert.ConditionSnapshot);

        return MapToDto(alert, isRead, isDismissed);
    }

    public Task<bool> PantryItemExistsInHouseholdAsync(
        Guid householdId,
        Guid pantryItemId,
        CancellationToken cancellationToken = default) =>
        _dbContext.PantryItems.AnyAsync(
            p => p.HouseholdId == householdId && p.Id == pantryItemId,
            cancellationToken);

    public async Task<InventoryAlertDto?> CreateAlertAsync(
        Guid householdId,
        Guid pantryItemId,
        AlertType alertType,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.PantryItems
            .Include(p => p.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == pantryItemId && p.HouseholdId == householdId, cancellationToken);

        if (item is null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var candidate = InventoryAlertEvaluator.Evaluate(item, today)
            .FirstOrDefault(c => c.AlertType == alertType);

        if (candidate is null)
        {
            return null;
        }

        var alert = await UpsertAlertAsync(householdId, item, candidate, cancellationToken);
        return MapToDto(alert, isRead: false, isDismissed: false);
    }

    public async Task<bool> MarkReadAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.InventoryAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.HouseholdId == householdId && a.IsActive, cancellationToken);

        if (alert is null)
        {
            return false;
        }

        var existing = await _dbContext.InventoryAlertReads
            .FirstOrDefaultAsync(r => r.UserId == userId && r.AlertId == alertId, cancellationToken);

        if (existing is not null)
        {
            existing.ReadAt = DateTimeOffset.UtcNow;
        }
        else
        {
            _dbContext.InventoryAlertReads.Add(new InventoryAlertRead
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AlertId = alertId,
                ReadAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DismissAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.InventoryAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == alertId && a.HouseholdId == householdId && a.IsActive, cancellationToken);

        if (alert is null)
        {
            return false;
        }

        var existing = await _dbContext.InventoryAlertDismissals
            .FirstOrDefaultAsync(
                d => d.UserId == userId
                    && d.PantryItemId == alert.PantryItemId
                    && d.AlertType == alert.AlertType,
                cancellationToken);

        var utcNow = DateTimeOffset.UtcNow;
        if (existing is not null)
        {
            existing.DismissedAt = utcNow;
            existing.ConditionSnapshot = alert.ConditionSnapshot;
        }
        else
        {
            _dbContext.InventoryAlertDismissals.Add(new InventoryAlertDismissal
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PantryItemId = alert.PantryItemId,
                AlertType = alert.AlertType,
                DismissedAt = utcNow,
                ConditionSnapshot = alert.ConditionSnapshot,
                CreatedAt = utcNow
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<InventoryAlert> UpsertAlertAsync(
        Guid householdId,
        PantryItem item,
        InventoryAlertEvaluator.AlertCandidate candidate,
        CancellationToken cancellationToken)
    {
        var alert = await _dbContext.InventoryAlerts
            .FirstOrDefaultAsync(
                a => a.PantryItemId == item.Id && a.AlertType == candidate.AlertType,
                cancellationToken);

        if (alert is null)
        {
            alert = new InventoryAlert
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                PantryItemId = item.Id,
                AlertType = candidate.AlertType,
                Message = candidate.Message,
                Severity = candidate.Severity,
                ConditionSnapshot = candidate.ConditionSnapshot,
                IsActive = true
            };
            _dbContext.InventoryAlerts.Add(alert);
        }
        else
        {
            alert.HouseholdId = householdId;
            alert.Message = candidate.Message;
            alert.Severity = candidate.Severity;
            alert.ConditionSnapshot = candidate.ConditionSnapshot;
            alert.IsActive = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        alert.PantryItem = item;
        return alert;
    }

    private static InventoryAlertDto MapToDto(
        InventoryAlert alert,
        bool isRead,
        bool isDismissed) =>
        new()
        {
            Id = alert.Id,
            AlertType = AlertTypeHelper.ToApiValue(alert.AlertType),
            Message = alert.Message,
            Severity = alert.Severity,
            PantryItem = new InventoryAlertPantryItemDto
            {
                Id = alert.PantryItem.Id,
                IngredientId = alert.PantryItem.IngredientId,
                IngredientName = alert.PantryItem.Ingredient.Name,
                Quantity = alert.PantryItem.Quantity,
                Unit = alert.PantryItem.Unit,
                ExpirationDate = alert.PantryItem.ExpiryDate
            },
            IsRead = isRead,
            IsDismissed = isDismissed,
            CreatedAt = alert.CreatedAt,
            UpdatedAt = alert.UpdatedAt
        };
}
