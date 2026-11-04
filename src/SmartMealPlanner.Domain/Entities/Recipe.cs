using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Domain.Entities;

public class Recipe : HouseholdEntity, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Servings { get; set; } = 4;

    public int? PrepTimeMinutes { get; set; }

    public int? CookTimeMinutes { get; set; }

    public string? Instructions { get; set; }

    public Guid? CategoryId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public Household Household { get; set; } = null!;

    public RecipeCategory? Category { get; set; }

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public ICollection<RecipeStep> Steps { get; set; } = [];

    public ICollection<RecipeTag> Tags { get; set; } = [];
}
