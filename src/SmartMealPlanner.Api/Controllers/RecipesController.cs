using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Recipes.DTOs;
using SmartMealPlanner.Application.Recipes.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IValidator<CreateRecipeRequest> _createValidator;
    private readonly IValidator<UpdateRecipeRequest> _updateValidator;
    private readonly IValidator<RecipeListQuery> _listQueryValidator;

    public RecipesController(
        IRecipeService recipeService,
        IValidator<CreateRecipeRequest> createValidator,
        IValidator<UpdateRecipeRequest> updateValidator,
        IValidator<RecipeListQuery> listQueryValidator)
    {
        _recipeService = recipeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _listQueryValidator = listQueryValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PaginatedList<RecipeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PaginatedList<RecipeDto>>>> GetAll(
        [FromQuery] RecipeListQuery query,
        CancellationToken cancellationToken)
    {
        await _listQueryValidator.ValidateAndThrowAsync(query, cancellationToken);
        var result = await _recipeService.GetAllAsync(query, cancellationToken);
        return Ok(ApiResult<PaginatedList<RecipeDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<RecipeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<RecipeDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResult<RecipeDto>.Ok(recipe));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<RecipeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<RecipeDto>>> Create(
        [FromBody] CreateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var recipe = await _recipeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, ApiResult<RecipeDto>.Ok(recipe));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<RecipeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<RecipeDto>>> Update(
        Guid id,
        [FromBody] UpdateRecipeRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var recipe = await _recipeService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<RecipeDto>.Ok(recipe));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _recipeService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
