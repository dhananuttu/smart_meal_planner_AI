using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Todos.Helpers;

public static class TodoEnumHelper
{
    public static TodoCategory ParseCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category is required.");
        }

        var normalized = category.Trim().Replace(" ", string.Empty);
        return Enum.TryParse<TodoCategory>(normalized, ignoreCase: true, out var value)
            ? value
            : throw new ArgumentException($"'{category}' is not a valid todo category.");
    }

    public static TodoPriority ParsePriority(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("Priority is required.");
        }

        return Enum.TryParse<TodoPriority>(priority.Trim(), ignoreCase: true, out var value)
            ? value
            : throw new ArgumentException($"'{priority}' is not a valid todo priority.");
    }

    public static RecurrenceType? ParseRecurrenceType(string? recurrenceType)
    {
        if (string.IsNullOrWhiteSpace(recurrenceType))
        {
            return null;
        }

        return Enum.TryParse<RecurrenceType>(recurrenceType.Trim(), ignoreCase: true, out var value)
            ? value
            : throw new ArgumentException($"'{recurrenceType}' is not a valid recurrence type.");
    }

    public static string ToApiValue(TodoCategory category) => category switch
    {
        TodoCategory.MealPreparation => "Meal Preparation",
        _ => category.ToString()
    };

    public static string ToApiValue(TodoPriority priority) => priority.ToString();

    public static string? ToApiValue(RecurrenceType? recurrenceType) =>
        recurrenceType?.ToString();

    public static bool IsValidCategory(string category)
    {
        try
        {
            ParseCategory(category);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static bool IsValidPriority(string priority)
    {
        try
        {
            ParsePriority(priority);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static bool IsValidRecurrenceType(string recurrenceType)
    {
        try
        {
            ParseRecurrenceType(recurrenceType);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
