using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.DTOs;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/recipe-categories")]
public class RecipeCategoriesController : ControllerBase
{
    private readonly IRecipeCategoryService _recipeCategoryService;
    private readonly IValidator<CreateRecipeCategoryRequest> _createValidator;
    private readonly IValidator<UpdateRecipeCategoryRequest> _updateValidator;
    private readonly IValidator<RecipeCategoryListQuery> _listQueryValidator;

    public RecipeCategoriesController(
        IRecipeCategoryService recipeCategoryService,
        IValidator<CreateRecipeCategoryRequest> createValidator,
        IValidator<UpdateRecipeCategoryRequest> updateValidator,
        IValidator<RecipeCategoryListQuery> listQueryValidator)
    {
        _recipeCategoryService = recipeCategoryService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _listQueryValidator = listQueryValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PaginatedList<RecipeCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PaginatedList<RecipeCategoryDto>>>> GetAll(
        [FromQuery] RecipeCategoryListQuery query,
        CancellationToken cancellationToken)
    {
        await _listQueryValidator.ValidateAndThrowAsync(query, cancellationToken);
        var result = await _recipeCategoryService.GetAllAsync(query, cancellationToken);
        return Ok(ApiResult<PaginatedList<RecipeCategoryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<RecipeCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<RecipeCategoryDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _recipeCategoryService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResult<RecipeCategoryDto>.Ok(category));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<RecipeCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResult<RecipeCategoryDto>>> Create(
        [FromBody] CreateRecipeCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var category = await _recipeCategoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, ApiResult<RecipeCategoryDto>.Ok(category));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<RecipeCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResult<RecipeCategoryDto>>> Update(
        Guid id,
        [FromBody] UpdateRecipeCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var category = await _recipeCategoryService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<RecipeCategoryDto>.Ok(category));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _recipeCategoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
