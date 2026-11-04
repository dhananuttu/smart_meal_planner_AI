using FluentAssertions;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class InventoryAlertEvaluatorTests
{
    [Fact]
    public void Evaluate_OutOfStock_TakesPrecedenceOverLowStock()
    {
        var item = CreateItem(quantity: 0, threshold: 5, expiry: null);
        var alerts = InventoryAlertEvaluator.Evaluate(item, DateOnly.FromDateTime(DateTime.UtcNow));

        alerts.Should().ContainSingle(a => a.AlertType == AlertType.OutOfStock);
        alerts.Should().NotContain(a => a.AlertType == AlertType.LowStock);
    }

    [Fact]
    public void Evaluate_ExpiredItem_ReturnsExpiredAlert()
    {
        var item = CreateItem(quantity: 2, threshold: 1, expiry: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)));
        var alerts = InventoryAlertEvaluator.Evaluate(item, DateOnly.FromDateTime(DateTime.UtcNow));

        alerts.Should().Contain(a => a.AlertType == AlertType.Expired);
    }

    private static PantryItem CreateItem(decimal quantity, decimal? threshold, DateOnly? expiry) =>
        new()
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            Unit = "kg",
            LowStockThreshold = threshold,
            ExpiryDate = expiry,
            Ingredient = new Ingredient { Name = "Spinach" }
        };
}
