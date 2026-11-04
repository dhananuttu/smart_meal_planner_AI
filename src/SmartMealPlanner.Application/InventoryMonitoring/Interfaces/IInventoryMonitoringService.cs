using SmartMealPlanner.Application.InventoryMonitoring.Models;

namespace SmartMealPlanner.Application.InventoryMonitoring.Interfaces;

/// <summary>
/// Evaluates pantry inventory and generates alerts.
/// Designed for API-triggered refresh and future background schedulers
/// (e.g. IHostedService, Hangfire, or cloud cron).
/// </summary>
public interface IInventoryMonitoringService
{
    /// <summary>
    /// Monitors a single household pantry and upserts/deactivates alerts.
    /// </summary>
    Task<InventoryMonitoringResult> MonitorHouseholdAsync(
        Guid householdId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitors all households. Intended for batch/scheduled execution.
    /// </summary>
    Task<IReadOnlyList<InventoryMonitoringResult>> MonitorAllHouseholdsAsync(
        CancellationToken cancellationToken = default);
}
