using SmartMealPlanner.Application.InventoryAlerts.DTOs;

namespace SmartMealPlanner.Application.InventoryAlerts.Interfaces;

public interface IInventoryAlertRepository
{
    Task<IReadOnlyList<InventoryAlertDto>> GetAlertsAsync(
        Guid householdId,
        Guid userId,
        InventoryAlertListQuery query,
        CancellationToken cancellationToken = default);

    Task<InventoryAlertDto?> GetByIdAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default);

    Task<bool> PantryItemExistsInHouseholdAsync(
        Guid householdId,
        Guid pantryItemId,
        CancellationToken cancellationToken = default);

    Task<InventoryAlertDto?> CreateAlertAsync(
        Guid householdId,
        Guid pantryItemId,
        Domain.Enums.AlertType alertType,
        CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default);

    Task<bool> DismissAsync(
        Guid householdId,
        Guid userId,
        Guid alertId,
        CancellationToken cancellationToken = default);
}
