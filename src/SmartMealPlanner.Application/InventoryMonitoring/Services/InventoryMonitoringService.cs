using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Models;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.InventoryMonitoring.Services;

public class InventoryMonitoringService : IInventoryMonitoringService
{
    private readonly IInventoryMonitoringRepository _repository;
    private readonly IDateTimeService _dateTimeService;

    public InventoryMonitoringService(
        IInventoryMonitoringRepository repository,
        IDateTimeService dateTimeService)
    {
        _repository = repository;
        _dateTimeService = dateTimeService;
    }

    public async Task<InventoryMonitoringResult> MonitorHouseholdAsync(
        Guid householdId,
        CancellationToken cancellationToken = default)
    {
        var today = _dateTimeService.TodayUtc;
        var pantryItems = await _repository.GetPantryItemsAsync(householdId, cancellationToken);

        var activeKeys = new HashSet<(Guid PantryItemId, AlertType AlertType)>();
        var generatedAlerts = new List<MonitoredAlertSummary>();
        var created = 0;
        var updated = 0;

        foreach (var item in pantryItems)
        {
            foreach (var candidate in InventoryAlertEvaluator.Evaluate(item, today))
            {
                activeKeys.Add((item.PantryItemId, candidate.AlertType));

                var outcome = await _repository.UpsertAlertAsync(
                    householdId,
                    item.PantryItemId,
                    candidate,
                    cancellationToken);

                switch (outcome)
                {
                    case AlertUpsertOutcome.Created:
                        created++;
                        break;
                    case AlertUpsertOutcome.Updated:
                        updated++;
                        break;
                }

                generatedAlerts.Add(new MonitoredAlertSummary
                {
                    PantryItemId = item.PantryItemId,
                    AlertType = AlertTypeHelper.ToApiValue(candidate.AlertType),
                    Message = candidate.Message,
                    Severity = candidate.Severity
                });
            }
        }

        var deactivated = await _repository.DeactivateStaleAlertsAsync(
            householdId,
            activeKeys,
            cancellationToken);

        return new InventoryMonitoringResult
        {
            HouseholdId = householdId,
            EvaluatedOn = today,
            PantryItemsScanned = pantryItems.Count,
            AlertsCreated = created,
            AlertsUpdated = updated,
            AlertsDeactivated = deactivated,
            GeneratedAlerts = generatedAlerts
        };
    }

    public async Task<IReadOnlyList<InventoryMonitoringResult>> MonitorAllHouseholdsAsync(
        CancellationToken cancellationToken = default)
    {
        var householdIds = await _repository.GetHouseholdIdsAsync(cancellationToken);
        var results = new List<InventoryMonitoringResult>(householdIds.Count);

        foreach (var householdId in householdIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            results.Add(await MonitorHouseholdAsync(householdId, cancellationToken));
        }

        return results;
    }
}
