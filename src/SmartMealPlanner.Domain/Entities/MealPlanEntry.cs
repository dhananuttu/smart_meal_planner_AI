using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class MealPlanEntry : AuditableEntity
{
    public Guid MealPlanId { get; set; }

    public Guid? RecipeId { get; set; }

    public DateOnly EntryDate { get; set; }

    public MealSlot MealSlot { get; set; }

    public string? Note { get; set; }

    public int SortOrder { get; set; }

    public MealPlan MealPlan { get; set; } = null!;

    public Recipe? Recipe { get; set; }
}
