using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryMonitoring.Models;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.InventoryMonitoring.Interfaces;

/// <summary>
/// Persistence port for inventory monitoring runs.
/// Keeps monitoring logic independent of HTTP and schedulers.
/// </summary>
public interface IInventoryMonitoringRepository
{
    Task<IReadOnlyList<Guid>> GetHouseholdIdsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PantryItemMonitoringSnapshot>> GetPantryItemsAsync(
        Guid householdId,
        CancellationToken cancellationToken = default);

    Task<AlertUpsertOutcome> UpsertAlertAsync(
        Guid householdId,
        Guid pantryItemId,
        InventoryAlertEvaluator.AlertCandidate candidate,
        CancellationToken cancellationToken = default);

    Task<int> DeactivateStaleAlertsAsync(
        Guid householdId,
        IReadOnlyCollection<(Guid PantryItemId, AlertType AlertType)> activeKeys,
        CancellationToken cancellationToken = default);
}
