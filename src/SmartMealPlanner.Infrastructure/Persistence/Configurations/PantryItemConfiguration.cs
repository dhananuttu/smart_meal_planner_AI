using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class PantryItemConfiguration : IEntityTypeConfiguration<PantryItem>
{
    public void Configure(EntityTypeBuilder<PantryItem> builder)
    {
        builder.ToTable("pantry_items");
        builder.ConfigureHouseholdEntity();

        builder.Property(i => i.Quantity)
            .HasPrecision(10, 2)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_pantry_items_quantity",
            "quantity >= 0");

        builder.Property(i => i.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.LowStockThreshold)
            .HasPrecision(10, 2);

        builder.HasCheckConstraint(
            "ck_pantry_items_low_stock_threshold",
            "low_stock_threshold IS NULL OR low_stock_threshold >= 0");

        builder.HasIndex(i => new { i.HouseholdId, i.IngredientId })
            .IsUnique();

        builder.HasIndex(i => new { i.HouseholdId, i.ExpiryDate })
            .HasFilter("expiry_date IS NOT NULL");

        builder.HasIndex(i => new { i.HouseholdId, i.Quantity, i.LowStockThreshold })
            .HasFilter("low_stock_threshold IS NOT NULL");

        builder.HasOne(i => i.Household)
            .WithMany()
            .HasForeignKey(i => i.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Ingredient)
            .WithMany()
            .HasForeignKey(i => i.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
