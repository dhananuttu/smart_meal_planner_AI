using SmartMealPlanner.Application.InventoryAlerts.DTOs;

namespace SmartMealPlanner.Application.InventoryAlerts.Interfaces;

public interface IInventoryAlertService
{
    Task<IReadOnlyList<InventoryAlertDto>> GetAlertsAsync(
        InventoryAlertListQuery query,
        CancellationToken cancellationToken = default);

    Task<InventoryAlertDto> CreateAlertAsync(
        CreateInventoryAlertRequest request,
        CancellationToken cancellationToken = default);

    Task MarkReadAsync(Guid alertId, CancellationToken cancellationToken = default);

    Task DismissAlertAsync(Guid alertId, CancellationToken cancellationToken = default);
}
