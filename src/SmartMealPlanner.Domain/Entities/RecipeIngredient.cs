using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class RecipeIngredient : AuditableEntity
{
    public Guid RecipeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
