using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class GroceryList : HouseholdEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid? MealPlanId { get; set; }

    public bool IsArchived { get; set; }

    public Household Household { get; set; } = null!;

    public MealPlan? MealPlan { get; set; }

    public ICollection<GroceryListItem> Items { get; set; } = [];
}
