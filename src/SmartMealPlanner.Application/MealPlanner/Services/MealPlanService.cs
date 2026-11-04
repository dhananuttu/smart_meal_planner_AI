using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.MealPlanner.DTOs;
using SmartMealPlanner.Application.MealPlanner.Helpers;
using SmartMealPlanner.Application.MealPlanner.Interfaces;
using SmartMealPlanner.Domain.Exceptions;
using DomainValidationException = SmartMealPlanner.Domain.Exceptions.ValidationException;

namespace SmartMealPlanner.Application.MealPlanner.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IHouseholdContext _householdContext;

    public MealPlanService(
        IMealPlanRepository mealPlanRepository,
        IHouseholdContext householdContext)
    {
        _mealPlanRepository = mealPlanRepository;
        _householdContext = householdContext;
    }

    public async Task<WeeklyMealPlanDto> GetWeeklyPlanAsync(
        DateOnly weekStart,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var normalizedWeekStart = MealPlannerDateHelper.GetWeekStartMonday(weekStart);
        return await _mealPlanRepository.GetOrCreateWeeklyPlanAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            normalizedWeekStart,
            cancellationToken);
    }

    public async Task<MealDto> CreateMealAsync(
        CreateMealRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        await ValidateRecipeAsync(request.RecipeId, cancellationToken);

        var weekStart = MealPlannerDateHelper.GetWeekStartMonday(request.Date);
        if (!MealPlannerDateHelper.IsDateInWeek(request.Date, weekStart))
        {
            throw new DomainValidationException("Date must fall within a valid weekly meal plan.");
        }

        return await _mealPlanRepository.CreateMealAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            request.Date,
            MealTypeHelper.ToApiValue(MealTypeHelper.Parse(request.MealType)),
            request.RecipeId,
            string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            cancellationToken);
    }

    public async Task<MealDto> UpdateMealAsync(
        Guid mealId,
        UpdateMealRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        await ValidateRecipeAsync(request.RecipeId, cancellationToken);

        return await _mealPlanRepository.UpdateMealAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            mealId,
            request.Date,
            MealTypeHelper.ToApiValue(MealTypeHelper.Parse(request.MealType)),
            request.RecipeId,
            string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            cancellationToken)
            ?? throw new NotFoundException("Meal", mealId);
    }

    public async Task DeleteMealAsync(Guid mealId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var deleted = await _mealPlanRepository.DeleteMealAsync(
            _householdContext.HouseholdId,
            mealId,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Meal", mealId);
        }
    }

    private async Task ValidateRecipeAsync(Guid? recipeId, CancellationToken cancellationToken)
    {
        if (!recipeId.HasValue)
        {
            return;
        }

        var exists = await _mealPlanRepository.RecipeExistsInHouseholdAsync(
            _householdContext.HouseholdId,
            recipeId.Value,
            cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Recipe", recipeId.Value);
        }
    }
}
