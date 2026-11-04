using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class MealPlanEntryConfiguration : IEntityTypeConfiguration<MealPlanEntry>
{
    public void Configure(EntityTypeBuilder<MealPlanEntry> builder)
    {
        builder.ToTable("meal_plan_entries");
        builder.ConfigureAuditableEntity();

        builder.Property(e => e.MealSlot)
            .HasConversion(
                slot => slot.ToString(),
                value => Enum.Parse<MealSlot>(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_meal_plan_entries_meal_slot",
            "meal_slot IN ('Breakfast', 'Lunch', 'Dinner', 'Snack')");

        builder.Property(e => e.Note)
            .HasMaxLength(500);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(e => e.MealPlanId);
        builder.HasIndex(e => new { e.MealPlanId, e.EntryDate, e.MealSlot });
        builder.HasIndex(e => e.RecipeId);
        builder.HasIndex(e => new { e.EntryDate, e.MealSlot });

        builder.HasOne(e => e.MealPlan)
            .WithMany(p => p.Entries)
            .HasForeignKey(e => e.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Recipe)
            .WithMany()
            .HasForeignKey(e => e.RecipeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
