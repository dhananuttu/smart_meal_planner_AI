namespace SmartMealPlanner.Domain.Entities;

public class RecipeTag
{
    public Guid Id { get; set; }

    public Guid RecipeId { get; set; }

    public string Tag { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
