using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Models;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class InventoryMonitoringRepository : IInventoryMonitoringRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryMonitoringRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Guid>> GetHouseholdIdsAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Households
            .AsNoTracking()
            .Select(h => h.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PantryItemMonitoringSnapshot>> GetPantryItemsAsync(
        Guid householdId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.PantryItems
            .AsNoTracking()
            .Include(p => p.Ingredient)
            .Where(p => p.HouseholdId == householdId)
            .Select(p => new PantryItemMonitoringSnapshot
            {
                PantryItemId = p.Id,
                HouseholdId = p.HouseholdId,
                IngredientName = p.Ingredient.Name,
                Quantity = p.Quantity,
                Unit = p.Unit,
                LowStockThreshold = p.LowStockThreshold,
                ExpiryDate = p.ExpiryDate
            })
            .ToListAsync(cancellationToken);

    public async Task<AlertUpsertOutcome> UpsertAlertAsync(
        Guid householdId,
        Guid pantryItemId,
        InventoryAlertEvaluator.AlertCandidate candidate,
        CancellationToken cancellationToken = default)
    {
        var alert = await _dbContext.InventoryAlerts
            .FirstOrDefaultAsync(
                a => a.PantryItemId == pantryItemId && a.AlertType == candidate.AlertType,
                cancellationToken);

        if (alert is null)
        {
            _dbContext.InventoryAlerts.Add(new InventoryAlert
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                PantryItemId = pantryItemId,
                AlertType = candidate.AlertType,
                Message = candidate.Message,
                Severity = candidate.Severity,
                ConditionSnapshot = candidate.ConditionSnapshot,
                IsActive = true
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
            return AlertUpsertOutcome.Created;
        }

        var changed = !alert.IsActive
            || alert.Message != candidate.Message
            || alert.Severity != candidate.Severity
            || alert.ConditionSnapshot != candidate.ConditionSnapshot;

        if (!changed)
        {
            return AlertUpsertOutcome.Unchanged;
        }

        alert.HouseholdId = householdId;
        alert.Message = candidate.Message;
        alert.Severity = candidate.Severity;
        alert.ConditionSnapshot = candidate.ConditionSnapshot;
        alert.IsActive = true;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return AlertUpsertOutcome.Updated;
    }

    public async Task<int> DeactivateStaleAlertsAsync(
        Guid householdId,
        IReadOnlyCollection<(Guid PantryItemId, AlertType AlertType)> activeKeys,
        CancellationToken cancellationToken = default)
    {
        var activeKeySet = activeKeys.ToHashSet();
        var existingAlerts = await _dbContext.InventoryAlerts
            .Where(a => a.HouseholdId == householdId && a.IsActive)
            .ToListAsync(cancellationToken);

        var deactivated = 0;
        foreach (var alert in existingAlerts)
        {
            if (!activeKeySet.Contains((alert.PantryItemId, alert.AlertType)))
            {
                alert.IsActive = false;
                deactivated++;
            }
        }

        if (deactivated > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return deactivated;
    }
}
