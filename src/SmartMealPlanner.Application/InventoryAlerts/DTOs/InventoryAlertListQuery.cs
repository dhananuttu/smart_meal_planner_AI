namespace SmartMealPlanner.Application.InventoryAlerts.DTOs;

public class InventoryAlertListQuery
{
    public bool IncludeDismissed { get; set; }

    public bool IncludeRead { get; set; } = true;
}
