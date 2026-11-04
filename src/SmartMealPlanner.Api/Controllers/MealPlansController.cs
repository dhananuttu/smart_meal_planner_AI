using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.MealPlanner.DTOs;
using SmartMealPlanner.Application.MealPlanner.Interfaces;

namespace SmartMealPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/meal-plans")]
public class MealPlansController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;
    private readonly IValidator<CreateMealRequest> _createValidator;
    private readonly IValidator<UpdateMealRequest> _updateValidator;

    public MealPlansController(
        IMealPlanService mealPlanService,
        IValidator<CreateMealRequest> createValidator,
        IValidator<UpdateMealRequest> updateValidator)
    {
        _mealPlanService = mealPlanService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("weekly")]
    [ProducesResponseType(typeof(ApiResult<WeeklyMealPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<WeeklyMealPlanDto>>> GetWeeklyPlan(
        [FromQuery] DateOnly weekStart,
        CancellationToken cancellationToken)
    {
        var plan = await _mealPlanService.GetWeeklyPlanAsync(weekStart, cancellationToken);
        return Ok(ApiResult<WeeklyMealPlanDto>.Ok(plan));
    }

    [HttpPost("meals")]
    [ProducesResponseType(typeof(ApiResult<MealDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<MealDto>>> CreateMeal(
        [FromBody] CreateMealRequest request,
        CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);
        var meal = await _mealPlanService.CreateMealAsync(request, cancellationToken);
        return Created(string.Empty, ApiResult<MealDto>.Ok(meal));
    }

    [HttpPut("meals/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<MealDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<MealDto>>> UpdateMeal(
        Guid id,
        [FromBody] UpdateMealRequest request,
        CancellationToken cancellationToken)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        var meal = await _mealPlanService.UpdateMealAsync(id, request, cancellationToken);
        return Ok(ApiResult<MealDto>.Ok(meal));
    }

    [HttpDelete("meals/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMeal(Guid id, CancellationToken cancellationToken)
    {
        await _mealPlanService.DeleteMealAsync(id, cancellationToken);
        return NoContent();
    }
}
