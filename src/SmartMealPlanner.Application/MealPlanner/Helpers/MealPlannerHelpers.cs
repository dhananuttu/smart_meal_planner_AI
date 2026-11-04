using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.MealPlanner.Helpers;

public static class MealPlannerDateHelper
{
    public static DateOnly GetWeekStartMonday(DateOnly date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-daysFromMonday);
    }

    public static DateOnly GetWeekEndSunday(DateOnly weekStartMonday) =>
        weekStartMonday.AddDays(6);

    public static bool IsDateInWeek(DateOnly date, DateOnly weekStartMonday)
    {
        var weekEnd = GetWeekEndSunday(weekStartMonday);
        return date >= weekStartMonday && date <= weekEnd;
    }
}

public static class MealTypeHelper
{
    public static MealSlot Parse(string mealType)
    {
        if (string.IsNullOrWhiteSpace(mealType))
        {
            throw new ArgumentException("Meal type is required.");
        }

        return Enum.TryParse<MealSlot>(mealType.Trim(), ignoreCase: true, out var slot)
            ? slot
            : throw new ArgumentException($"'{mealType}' is not a valid meal type.");
    }

    public static string ToApiValue(MealSlot slot) =>
        slot.ToString();
}
