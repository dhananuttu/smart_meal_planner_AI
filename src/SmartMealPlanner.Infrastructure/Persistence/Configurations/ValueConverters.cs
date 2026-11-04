using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

internal static class ValueConverters
{
    public static string HouseholdRoleToString(HouseholdRole role) =>
        role == HouseholdRole.Owner ? "owner" : "member";

    public static HouseholdRole HouseholdRoleFromString(string value) =>
        value == "owner" ? HouseholdRole.Owner : HouseholdRole.Member;

    public static string AlertTypeToString(AlertType type) =>
        type switch
        {
            AlertType.LowStock => "low_stock",
            AlertType.OutOfStock => "out_of_stock",
            AlertType.ExpiringSoon => "expiring_soon",
            AlertType.Expired => "expired",
            _ => "low_stock"
        };

    public static AlertType AlertTypeFromString(string value) =>
        value switch
        {
            "low_stock" => AlertType.LowStock,
            "out_of_stock" => AlertType.OutOfStock,
            "expiring_soon" => AlertType.ExpiringSoon,
            "expired" => AlertType.Expired,
            // Legacy values from earlier schema
            "expiring" => AlertType.ExpiringSoon,
            _ => AlertType.LowStock
        };
}
