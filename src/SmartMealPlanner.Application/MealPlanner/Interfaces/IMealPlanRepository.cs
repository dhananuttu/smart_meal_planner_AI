using SmartMealPlanner.Application.MealPlanner.DTOs;

namespace SmartMealPlanner.Application.MealPlanner.Interfaces;

public interface IMealPlanRepository
{
    Task<WeeklyMealPlanDto> GetOrCreateWeeklyPlanAsync(
        Guid householdId,
        Guid userId,
        DateOnly weekStartDate,
        CancellationToken cancellationToken = default);

    Task<MealDto?> GetMealByIdAsync(
        Guid householdId,
        Guid mealId,
        CancellationToken cancellationToken = default);

    Task<bool> RecipeExistsInHouseholdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default);

    Task<MealDto> CreateMealAsync(
        Guid householdId,
        Guid userId,
        DateOnly date,
        string mealType,
        Guid? recipeId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<MealDto?> UpdateMealAsync(
        Guid householdId,
        Guid userId,
        Guid mealId,
        DateOnly date,
        string mealType,
        Guid? recipeId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteMealAsync(
        Guid householdId,
        Guid mealId,
        CancellationToken cancellationToken = default);
}
