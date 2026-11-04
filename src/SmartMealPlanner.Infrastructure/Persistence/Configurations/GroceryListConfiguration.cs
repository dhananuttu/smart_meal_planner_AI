using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class GroceryListConfiguration : IEntityTypeConfiguration<GroceryList>
{
    public void Configure(EntityTypeBuilder<GroceryList> builder)
    {
        builder.ToTable("grocery_lists");
        builder.ConfigureHouseholdEntity();

        builder.Property(l => l.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.IsArchived)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(l => new { l.HouseholdId, l.IsArchived, l.CreatedAt });
        builder.HasIndex(l => l.MealPlanId);

        builder.HasOne(l => l.Household)
            .WithMany()
            .HasForeignKey(l => l.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.MealPlan)
            .WithMany()
            .HasForeignKey(l => l.MealPlanId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
