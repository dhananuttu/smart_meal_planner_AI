namespace SmartMealPlanner.Application.MealPlanner.DTOs;

public class DailyMealsDto
{
    public DateOnly Date { get; set; }

    public IReadOnlyList<MealDto> Meals { get; set; } = [];
}
