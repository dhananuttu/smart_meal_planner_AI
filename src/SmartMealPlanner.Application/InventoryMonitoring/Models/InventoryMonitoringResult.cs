namespace SmartMealPlanner.Application.InventoryMonitoring.Models;

public class InventoryMonitoringResult
{
    public Guid HouseholdId { get; init; }

    public DateOnly EvaluatedOn { get; init; }

    public int PantryItemsScanned { get; init; }

    public int AlertsCreated { get; init; }

    public int AlertsUpdated { get; init; }

    public int AlertsDeactivated { get; init; }

    public IReadOnlyList<MonitoredAlertSummary> GeneratedAlerts { get; init; } = [];
}
