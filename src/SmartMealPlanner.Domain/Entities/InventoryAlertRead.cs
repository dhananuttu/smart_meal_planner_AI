namespace SmartMealPlanner.Domain.Entities;

public class InventoryAlertRead
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid AlertId { get; set; }

    public DateTimeOffset ReadAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public InventoryAlert Alert { get; set; } = null!;
}
