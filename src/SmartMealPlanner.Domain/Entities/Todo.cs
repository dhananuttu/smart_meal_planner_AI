using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Domain.Entities;

public class Todo : UserOwnedEntity, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoCategory Category { get; set; } = TodoCategory.Personal;

    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    public DateOnly? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public bool IsRecurring { get; set; }

    public RecurrenceType? RecurrenceType { get; set; }

    public Guid? MealPlanEntryId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public MealPlanEntry? MealPlanEntry { get; set; }
}
