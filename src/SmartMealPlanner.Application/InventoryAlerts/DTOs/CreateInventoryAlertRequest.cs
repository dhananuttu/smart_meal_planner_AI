namespace SmartMealPlanner.Application.InventoryAlerts.DTOs;

public class CreateInventoryAlertRequest
{
    public Guid PantryItemId { get; set; }

    public string AlertType { get; set; } = string.Empty;
}
