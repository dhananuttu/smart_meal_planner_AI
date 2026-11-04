using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Application.Ingredients.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;
    private readonly IValidator<CreateIngredientRequest> _createValidator;
    private readonly IValidator<UpdateIngredientRequest> _updateValidator;
    private readonly IValidator<IngredientListQuery> _listQueryValidator;

    public IngredientsController(
        IIngredientService ingredientService,
        IValidator<CreateIngredientRequest> createValidator,
        IValidator<UpdateIngredientRequest> updateValidator,
        IValidator<IngredientListQuery> listQueryValidator)
    {
        _ingredientService = ingredientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _listQueryValidator = listQueryValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PaginatedList<IngredientDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PaginatedList<IngredientDto>>>> GetAll(
        [FromQuery] IngredientListQuery query,
        CancellationToken cancellationToken)
    {
        await _listQueryValidator.ValidateAndThrowAsync(query, cancellationToken);
        var result = await _ingredientService.GetAllAsync(query, cancellationToken);
        return Ok(ApiResult<PaginatedList<IngredientDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<IngredientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<IngredientDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResult<IngredientDto>.Ok(ingredient));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<IngredientDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResult<IngredientDto>>> Create(
        [FromBody] CreateIngredientRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var ingredient = await _ingredientService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ApiResult<IngredientDto>.Ok(ingredient));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<IngredientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResult<IngredientDto>>> Update(
        Guid id,
        [FromBody] UpdateIngredientRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var ingredient = await _ingredientService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<IngredientDto>.Ok(ingredient));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _ingredientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
