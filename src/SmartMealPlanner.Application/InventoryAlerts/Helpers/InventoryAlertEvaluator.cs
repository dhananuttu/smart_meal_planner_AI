using SmartMealPlanner.Application.InventoryMonitoring.Models;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.InventoryAlerts.Helpers;

public static class InventoryAlertEvaluator
{
    public const int ExpiringSoonDays = 7;

    public record AlertCandidate(
        AlertType AlertType,
        string Message,
        string Severity,
        string ConditionSnapshot);

    public static IReadOnlyList<AlertCandidate> Evaluate(PantryItem item, DateOnly today)
    {
        var candidates = new List<AlertCandidate>();

        if (item.Quantity == 0)
        {
            candidates.Add(new AlertCandidate(
                AlertType.OutOfStock,
                $"{item.Ingredient.Name} is out of stock.",
                "high",
                BuildQuantitySnapshot(item)));
        }
        else if (item.LowStockThreshold.HasValue && item.Quantity <= item.LowStockThreshold)
        {
            candidates.Add(new AlertCandidate(
                AlertType.LowStock,
                $"{item.Ingredient.Name} is running low ({item.Quantity} {item.Unit} remaining).",
                "medium",
                BuildQuantitySnapshot(item)));
        }

        if (item.ExpiryDate.HasValue)
        {
            if (item.ExpiryDate < today)
            {
                candidates.Add(new AlertCandidate(
                    AlertType.Expired,
                    $"{item.Ingredient.Name} expired on {item.ExpiryDate:yyyy-MM-dd}.",
                    "high",
                    BuildExpirySnapshot(item)));
            }
            else if (item.ExpiryDate <= today.AddDays(ExpiringSoonDays))
            {
                candidates.Add(new AlertCandidate(
                    AlertType.ExpiringSoon,
                    $"{item.Ingredient.Name} expires on {item.ExpiryDate:yyyy-MM-dd}.",
                    "medium",
                    BuildExpirySnapshot(item)));
            }
        }

        return candidates;
    }

    public static IReadOnlyList<AlertCandidate> Evaluate(PantryItemMonitoringSnapshot item, DateOnly today) =>
        Evaluate(new PantryItem
        {
            Quantity = item.Quantity,
            Unit = item.Unit,
            LowStockThreshold = item.LowStockThreshold,
            ExpiryDate = item.ExpiryDate,
            Ingredient = new Ingredient { Name = item.IngredientName }
        }, today);

    public static bool MatchesCondition(string storedSnapshot, string currentSnapshot) =>
        string.Equals(storedSnapshot, currentSnapshot, StringComparison.Ordinal);

    public static string BuildQuantitySnapshot(PantryItem item) =>
        $"qty={item.Quantity}/threshold={item.LowStockThreshold}";

    public static string BuildExpirySnapshot(PantryItem item) =>
        $"expiry={item.ExpiryDate:yyyy-MM-dd}";
}
