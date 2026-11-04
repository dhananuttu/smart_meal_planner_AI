using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class RecipeStep : AuditableEntity
{
    public Guid RecipeId { get; set; }

    public int StepNumber { get; set; }

    public string Instruction { get; set; } = string.Empty;

    public Recipe Recipe { get; set; } = null!;
}
