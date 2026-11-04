namespace SmartMealPlanner.Application.InventoryAlerts.Helpers;

public static class AlertTypeHelper
{
    public static string ToApiValue(Domain.Enums.AlertType type) =>
        type switch
        {
            Domain.Enums.AlertType.LowStock => "LowStock",
            Domain.Enums.AlertType.OutOfStock => "OutOfStock",
            Domain.Enums.AlertType.ExpiringSoon => "ExpiringSoon",
            Domain.Enums.AlertType.Expired => "Expired",
            _ => type.ToString()
        };

    public static Domain.Enums.AlertType Parse(string value) =>
        Enum.TryParse<Domain.Enums.AlertType>(value.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : throw new ArgumentException($"'{value}' is not a valid alert type.");
}
