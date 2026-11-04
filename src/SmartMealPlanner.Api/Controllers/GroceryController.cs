using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Grocery.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/grocery/lists")]
public class GroceryController : ControllerBase
{
    private readonly IGroceryListService _groceryListService;
    private readonly IValidator<CreateGroceryListRequest> _createValidator;
    private readonly IValidator<GenerateGroceryListFromMealPlanRequest> _generateValidator;
    private readonly IValidator<AddGroceryItemRequest> _addItemValidator;
    private readonly IValidator<AddGroceryItemsFromReceiptRequest> _receiptValidator;
    private readonly IValidator<MarkGroceryItemPurchasedRequest> _markPurchasedValidator;

    public GroceryController(
        IGroceryListService groceryListService,
        IValidator<CreateGroceryListRequest> createValidator,
        IValidator<GenerateGroceryListFromMealPlanRequest> generateValidator,
        IValidator<AddGroceryItemRequest> addItemValidator,
        IValidator<AddGroceryItemsFromReceiptRequest> receiptValidator,
        IValidator<MarkGroceryItemPurchasedRequest> markPurchasedValidator)
    {
        _groceryListService = groceryListService;
        _createValidator = createValidator;
        _generateValidator = generateValidator;
        _addItemValidator = addItemValidator;
        _receiptValidator = receiptValidator;
        _markPurchasedValidator = markPurchasedValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<GroceryListSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<GroceryListSummaryDto>>>> GetLists(
        CancellationToken cancellationToken)
    {
        var lists = await _groceryListService.GetListsAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<GroceryListSummaryDto>>.Ok(lists));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<GroceryListDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<GroceryListDetailDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var list = await _groceryListService.GetListByIdAsync(id, cancellationToken);
        return Ok(ApiResult<GroceryListDetailDto>.Ok(list));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<GroceryListDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<GroceryListDetailDto>>> CreateList(
        [FromBody] CreateGroceryListRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var list = await _groceryListService.CreateListAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, ApiResult<GroceryListDetailDto>.Ok(list));
    }

    [HttpPost("generate-from-meal-plan")]
    [ProducesResponseType(typeof(ApiResult<GroceryListDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<GroceryListDetailDto>>> GenerateFromMealPlan(
        [FromBody] GenerateGroceryListFromMealPlanRequest request,
        CancellationToken cancellationToken)
    {
        await _generateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var list = await _groceryListService.GenerateFromMealPlanAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, ApiResult<GroceryListDetailDto>.Ok(list));
    }

    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(ApiResult<GroceryItemDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<GroceryItemDto>>> AddItem(
        Guid id,
        [FromBody] AddGroceryItemRequest request,
        CancellationToken cancellationToken)
    {
        await _addItemValidator.ValidateAndThrowAsync(request, cancellationToken);
        var item = await _groceryListService.AddItemAsync(id, request, cancellationToken);
        return Created($"/api/grocery/lists/items/{item.Id}", ApiResult<GroceryItemDto>.Ok(item));
    }

    [HttpPost("{id:guid}/items/from-receipt")]
    [ProducesResponseType(typeof(ApiResult<GroceryListDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<GroceryListDetailDto>>> AddItemsFromReceipt(
        Guid id,
        [FromBody] AddGroceryItemsFromReceiptRequest request,
        CancellationToken cancellationToken)
    {
        await _receiptValidator.ValidateAndThrowAsync(request, cancellationToken);
        var list = await _groceryListService.AddItemsFromReceiptAsync(id, request, cancellationToken);
        return Ok(ApiResult<GroceryListDetailDto>.Ok(list));
    }

    [HttpPatch("items/{itemId:guid}/purchased")]
    [ProducesResponseType(typeof(ApiResult<GroceryItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<GroceryItemDto>>> MarkPurchased(
        Guid itemId,
        [FromBody] MarkGroceryItemPurchasedRequest request,
        CancellationToken cancellationToken)
    {
        await _markPurchasedValidator.ValidateAndThrowAsync(request, cancellationToken);
        var item = await _groceryListService.MarkItemPurchasedAsync(itemId, request, cancellationToken);
        return Ok(ApiResult<GroceryItemDto>.Ok(item));
    }

    [HttpDelete("items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid itemId, CancellationToken cancellationToken)
    {
        await _groceryListService.DeleteItemAsync(itemId, cancellationToken);
        return NoContent();
    }
}
