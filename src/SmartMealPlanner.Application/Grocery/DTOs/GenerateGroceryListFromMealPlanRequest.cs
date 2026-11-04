namespace SmartMealPlanner.Application.Grocery.DTOs;

public class GenerateGroceryListFromMealPlanRequest
{
    public Guid? MealPlanId { get; set; }

    public DateOnly? WeekStart { get; set; }

    public string? Name { get; set; }
}
