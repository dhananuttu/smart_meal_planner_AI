using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.InventoryAlerts.DTOs;
using SmartMealPlanner.Application.InventoryAlerts.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/inventory-alerts")]
public class InventoryAlertsController : ControllerBase
{
    private readonly IInventoryAlertService _inventoryAlertService;
    private readonly IValidator<CreateInventoryAlertRequest> _createValidator;

    public InventoryAlertsController(
        IInventoryAlertService inventoryAlertService,
        IValidator<CreateInventoryAlertRequest> createValidator)
    {
        _inventoryAlertService = inventoryAlertService;
        _createValidator = createValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<InventoryAlertDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<InventoryAlertDto>>>> GetAlerts(
        [FromQuery] InventoryAlertListQuery query,
        CancellationToken cancellationToken)
    {
        var alerts = await _inventoryAlertService.GetAlertsAsync(query, cancellationToken);
        return Ok(ApiResult<IReadOnlyList<InventoryAlertDto>>.Ok(alerts));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<InventoryAlertDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<InventoryAlertDto>>> CreateAlert(
        [FromBody] CreateInventoryAlertRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var alert = await _inventoryAlertService.CreateAlertAsync(request, cancellationToken);
        return Created(string.Empty, ApiResult<InventoryAlertDto>.Ok(alert));
    }

    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        await _inventoryAlertService.MarkReadAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/dismiss")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DismissAlert(Guid id, CancellationToken cancellationToken)
    {
        await _inventoryAlertService.DismissAlertAsync(id, cancellationToken);
        return NoContent();
    }
}
