using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class GroceryListItem : AuditableEntity
{
    public Guid GroceryListId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; } = 1;

    public string Unit { get; set; } = string.Empty;

    public bool IsChecked { get; set; }

    public Guid? SourceRecipeId { get; set; }

    public string? Note { get; set; }

    public int SortOrder { get; set; }

    public GroceryList GroceryList { get; set; } = null!;

    public Recipe? SourceRecipe { get; set; }
}
