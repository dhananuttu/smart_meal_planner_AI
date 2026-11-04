using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class MealPlan : HouseholdEntity
{
    public DateOnly WeekStartDate { get; set; }

    public Household Household { get; set; } = null!;

    public ICollection<MealPlanEntry> Entries { get; set; } = [];
}
