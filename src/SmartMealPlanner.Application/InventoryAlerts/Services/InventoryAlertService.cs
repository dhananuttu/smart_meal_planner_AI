using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.InventoryAlerts.DTOs;
using SmartMealPlanner.Application.InventoryAlerts.Helpers;
using SmartMealPlanner.Application.InventoryAlerts.Interfaces;
using SmartMealPlanner.Application.InventoryMonitoring.Interfaces;
using SmartMealPlanner.Domain.Exceptions;
using DomainValidationException = SmartMealPlanner.Domain.Exceptions.ValidationException;

namespace SmartMealPlanner.Application.InventoryAlerts.Services;

public class InventoryAlertService : IInventoryAlertService
{
    private readonly IInventoryAlertRepository _repository;
    private readonly IInventoryMonitoringService _monitoringService;
    private readonly IHouseholdContext _householdContext;
    private readonly ICurrentUserService _currentUserService;

    public InventoryAlertService(
        IInventoryAlertRepository repository,
        IInventoryMonitoringService monitoringService,
        IHouseholdContext householdContext,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _monitoringService = monitoringService;
        _householdContext = householdContext;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<InventoryAlertDto>> GetAlertsAsync(
        InventoryAlertListQuery query,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        var userId = GetCurrentUserId();

        await _monitoringService.MonitorHouseholdAsync(_householdContext.HouseholdId, cancellationToken);

        return await _repository.GetAlertsAsync(
            _householdContext.HouseholdId,
            userId,
            query,
            cancellationToken);
    }

    public async Task<InventoryAlertDto> CreateAlertAsync(
        CreateInventoryAlertRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        var userId = GetCurrentUserId();
        var alertType = AlertTypeHelper.Parse(request.AlertType);

        if (!await _repository.PantryItemExistsInHouseholdAsync(
            _householdContext.HouseholdId,
            request.PantryItemId,
            cancellationToken))
        {
            throw new NotFoundException("Pantry item", request.PantryItemId);
        }

        var alert = await _repository.CreateAlertAsync(
            _householdContext.HouseholdId,
            request.PantryItemId,
            alertType,
            cancellationToken);

        if (alert is null)
        {
            throw new DomainValidationException("Alert condition is not met for this pantry item.");
        }

        return alert;
    }

    public async Task MarkReadAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        var userId = GetCurrentUserId();

        var marked = await _repository.MarkReadAsync(
            _householdContext.HouseholdId,
            userId,
            alertId,
            cancellationToken);

        if (!marked)
        {
            throw new NotFoundException("Inventory alert", alertId);
        }
    }

    public async Task DismissAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        var userId = GetCurrentUserId();

        var dismissed = await _repository.DismissAsync(
            _householdContext.HouseholdId,
            userId,
            alertId,
            cancellationToken);

        if (!dismissed)
        {
            throw new NotFoundException("Inventory alert", alertId);
        }
    }

    private Guid GetCurrentUserId() =>
        _currentUserService.UserId
        ?? throw new UnauthorizedException("User is not authenticated.");
}
