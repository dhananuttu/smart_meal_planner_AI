using SmartMealPlanner.Application.MealPlanner.DTOs;

namespace SmartMealPlanner.Application.MealPlanner.Interfaces;

public interface IMealPlanService
{
    Task<WeeklyMealPlanDto> GetWeeklyPlanAsync(
        DateOnly weekStart,
        CancellationToken cancellationToken = default);

    Task<MealDto> CreateMealAsync(
        CreateMealRequest request,
        CancellationToken cancellationToken = default);

    Task<MealDto> UpdateMealAsync(
        Guid mealId,
        UpdateMealRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteMealAsync(Guid mealId, CancellationToken cancellationToken = default);
}
