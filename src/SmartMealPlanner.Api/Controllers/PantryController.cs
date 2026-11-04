using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Pantry.DTOs;
using SmartMealPlanner.Application.Pantry.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/pantry/items")]
public class PantryController : ControllerBase
{
    private readonly IPantryService _pantryService;
    private readonly IValidator<CreatePantryItemRequest> _createValidator;
    private readonly IValidator<UpdatePantryItemRequest> _updateValidator;
    private readonly IValidator<PantryInventoryQuery> _inventoryQueryValidator;

    public PantryController(
        IPantryService pantryService,
        IValidator<CreatePantryItemRequest> createValidator,
        IValidator<UpdatePantryItemRequest> updateValidator,
        IValidator<PantryInventoryQuery> inventoryQueryValidator)
    {
        _pantryService = pantryService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _inventoryQueryValidator = inventoryQueryValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PaginatedList<PantryItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PaginatedList<PantryItemDto>>>> GetInventory(
        [FromQuery] PantryInventoryQuery query,
        CancellationToken cancellationToken)
    {
        await _inventoryQueryValidator.ValidateAndThrowAsync(query, cancellationToken);
        var inventory = await _pantryService.GetInventoryAsync(query, cancellationToken);
        return Ok(ApiResult<PaginatedList<PantryItemDto>>.Ok(inventory));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PantryItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<PantryItemDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var item = await _pantryService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResult<PantryItemDto>.Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<PantryItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResult<PantryItemDto>>> AddItem(
        [FromBody] CreatePantryItemRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var item = await _pantryService.AddItemAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResult<PantryItemDto>.Ok(item));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<PantryItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<PantryItemDto>>> UpdateItem(
        Guid id,
        [FromBody] UpdatePantryItemRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var item = await _pantryService.UpdateItemAsync(id, request, cancellationToken);
        return Ok(ApiResult<PantryItemDto>.Ok(item));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken cancellationToken)
    {
        await _pantryService.DeleteItemAsync(id, cancellationToken);
        return NoContent();
    }
}
