using FluentAssertions;
using Moq;
using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Models;
using SmartMealPlanner.Application.InventoryMonitoring.Services;
using SmartMealPlanner.Domain.Enums;
using Xunit;

namespace SmartMealPlanner.Application.Tests;

public class InventoryMonitoringServiceTests
{
    private readonly Mock<IInventoryMonitoringRepository> _repository = new();
    private readonly Mock<IDateTimeService> _dateTimeService = new();
    private readonly InventoryMonitoringService _sut;
    private readonly Guid _householdId = Guid.NewGuid();
    private readonly DateOnly _today = new(2026, 6, 23);

    public InventoryMonitoringServiceTests()
    {
        _dateTimeService.Setup(x => x.TodayUtc).Returns(_today);
        _sut = new InventoryMonitoringService(_repository.Object, _dateTimeService.Object);
    }

    [Fact]
    public async Task MonitorHouseholdAsync_GeneratesLowStockAndOutOfStockAlerts()
    {
        var lowStockItemId = Guid.NewGuid();
        var outOfStockItemId = Guid.NewGuid();

        _repository
            .Setup(x => x.GetPantryItemsAsync(_householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PantryItemMonitoringSnapshot>
            {
                new()
                {
                    PantryItemId = lowStockItemId,
                    HouseholdId = _householdId,
                    IngredientName = "Rice",
                    Quantity = 1,
                    Unit = "kg",
                    LowStockThreshold = 2
                },
                new()
                {
                    PantryItemId = outOfStockItemId,
                    HouseholdId = _householdId,
                    IngredientName = "Milk",
                    Quantity = 0,
                    Unit = "l",
                    LowStockThreshold = 1
                }
            });

        _repository
            .Setup(x => x.UpsertAlertAsync(
                _householdId,
                It.IsAny<Guid>(),
                It.IsAny<InventoryAlertEvaluator.AlertCandidate>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(AlertUpsertOutcome.Created);

        _repository
            .Setup(x => x.DeactivateStaleAlertsAsync(
                _householdId,
                It.IsAny<IReadOnlyCollection<(Guid, AlertType)>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _sut.MonitorHouseholdAsync(_householdId);

        result.PantryItemsScanned.Should().Be(2);
        result.AlertsCreated.Should().Be(2);
        result.GeneratedAlerts.Should().Contain(a => a.AlertType == "LowStock" && a.PantryItemId == lowStockItemId);
        result.GeneratedAlerts.Should().Contain(a => a.AlertType == "OutOfStock" && a.PantryItemId == outOfStockItemId);
    }

    [Fact]
    public async Task MonitorHouseholdAsync_GeneratesExpiryAlerts()
    {
        var itemId = Guid.NewGuid();
        _repository
            .Setup(x => x.GetPantryItemsAsync(_householdId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PantryItemMonitoringSnapshot>
            {
                new()
                {
                    PantryItemId = itemId,
                    HouseholdId = _householdId,
                    IngredientName = "Yogurt",
                    Quantity = 2,
                    Unit = "each",
                    ExpiryDate = _today.AddDays(3)
                }
            });

        _repository
            .Setup(x => x.UpsertAlertAsync(
                _householdId,
                itemId,
                It.Is<InventoryAlertEvaluator.AlertCandidate>(c => c.AlertType == AlertType.ExpiringSoon),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(AlertUpsertOutcome.Created);

        _repository
            .Setup(x => x.DeactivateStaleAlertsAsync(
                _householdId,
                It.IsAny<IReadOnlyCollection<(Guid, AlertType)>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _sut.MonitorHouseholdAsync(_householdId);

        result.GeneratedAlerts.Should().ContainSingle(a => a.AlertType == "ExpiringSoon");
    }

    [Fact]
    public async Task MonitorAllHouseholdsAsync_MonitorsEachHousehold()
    {
        var householdA = Guid.NewGuid();
        var householdB = Guid.NewGuid();

        _repository
            .Setup(x => x.GetHouseholdIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { householdA, householdB });

        _repository
            .Setup(x => x.GetPantryItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PantryItemMonitoringSnapshot>());

        _repository
            .Setup(x => x.DeactivateStaleAlertsAsync(
                It.IsAny<Guid>(),
                It.IsAny<IReadOnlyCollection<(Guid, AlertType)>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var results = await _sut.MonitorAllHouseholdsAsync();

        results.Should().HaveCount(2);
        results.Select(r => r.HouseholdId).Should().BeEquivalentTo(new[] { householdA, householdB });
        _repository.Verify(x => x.GetPantryItemsAsync(householdA, It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(x => x.GetPantryItemsAsync(householdB, It.IsAny<CancellationToken>()), Times.Once);
    }
}
