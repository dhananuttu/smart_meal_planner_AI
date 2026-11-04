namespace SmartMealPlanner.Application.Users.Constants;

public static class DietaryPreferences
{
    public static readonly IReadOnlySet<string> AllowedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "vegetarian",
        "vegan",
        "gluten-free",
        "dairy-free",
        "nut-free",
        "halal",
        "kosher",
        "low-carb",
        "keto"
    };
}
