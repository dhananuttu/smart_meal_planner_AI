namespace SmartMealPlanner.Application.InventoryAlerts.DTOs;

public class InventoryAlertDto
{
    public Guid Id { get; set; }

    public string AlertType { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public InventoryAlertPantryItemDto PantryItem { get; set; } = null!;

    public bool IsRead { get; set; }

    public bool IsDismissed { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
