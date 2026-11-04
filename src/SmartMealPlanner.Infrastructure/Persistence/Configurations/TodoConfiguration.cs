using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("todos", t =>
        {
            t.HasCheckConstraint(
                "ck_todos_category",
                "category IN ('MealPreparation', 'Shopping', 'Household', 'Personal')");
            t.HasCheckConstraint(
                "ck_todos_priority",
                "priority IN ('Low', 'Medium', 'High', 'Critical')");
            t.HasCheckConstraint(
                "ck_todos_recurrence_type",
                "recurrence_type IS NULL OR recurrence_type IN ('Daily', 'Weekly', 'Monthly', 'Yearly')");
        });

        builder.ConfigureUserOwnedEntity();

        builder.Property(t => t.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(t => t.Category)
            .HasConversion(
                category => category.ToString(),
                value => Enum.Parse<TodoCategory>(value))
            .HasMaxLength(30)
            .HasDefaultValue(TodoCategory.Personal)
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion(
                priority => priority.ToString(),
                value => Enum.Parse<TodoPriority>(value))
            .HasMaxLength(20)
            .HasDefaultValue(TodoPriority.Medium)
            .IsRequired();

        builder.Property(t => t.IsCompleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(t => t.IsRecurring)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(t => t.RecurrenceType)
            .HasConversion(
                recurrence => recurrence.HasValue ? recurrence.Value.ToString() : null,
                value => string.IsNullOrEmpty(value) ? null : Enum.Parse<RecurrenceType>(value))
            .HasMaxLength(20);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(t => new { t.UserId, t.IsCompleted, t.IsDeleted });
        builder.HasIndex(t => new { t.UserId, t.DueDate })
            .HasFilter("due_date IS NOT NULL AND is_deleted = false");
        builder.HasIndex(t => new { t.UserId, t.IsRecurring, t.IsDeleted });
        builder.HasIndex(t => t.MealPlanEntryId);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.MealPlanEntry)
            .WithMany()
            .HasForeignKey(t => t.MealPlanEntryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
