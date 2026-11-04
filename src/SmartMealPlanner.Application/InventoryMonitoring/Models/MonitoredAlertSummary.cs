namespace SmartMealPlanner.Application.InventoryMonitoring.Models;

public class MonitoredAlertSummary
{
    public Guid PantryItemId { get; init; }

    public string AlertType { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public string Severity { get; init; } = string.Empty;
}
