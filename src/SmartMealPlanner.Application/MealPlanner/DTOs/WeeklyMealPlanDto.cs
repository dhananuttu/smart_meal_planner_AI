namespace SmartMealPlanner.Application.MealPlanner.DTOs;

public class WeeklyMealPlanDto
{
    public Guid PlanId { get; set; }

    public DateOnly WeekStartDate { get; set; }

    public DateOnly WeekEndDate { get; set; }

    public IReadOnlyList<DailyMealsDto> Days { get; set; } = [];
}
