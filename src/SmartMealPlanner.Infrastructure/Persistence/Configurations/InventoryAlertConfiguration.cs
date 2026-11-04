using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class InventoryAlertConfiguration : IEntityTypeConfiguration<InventoryAlert>
{
    public void Configure(EntityTypeBuilder<InventoryAlert> builder)
    {
        builder.ToTable("inventory_alerts");
        builder.ConfigureAuditableEntity();

        builder.Property(a => a.AlertType)
            .HasConversion(
                type => ValueConverters.AlertTypeToString(type),
                value => ValueConverters.AlertTypeFromString(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_inventory_alerts_alert_type",
            "alert_type IN ('low_stock', 'out_of_stock', 'expiring_soon', 'expired')"));

        builder.Property(a => a.Message)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Severity)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.ConditionSnapshot)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(a => new { a.HouseholdId, a.IsActive });
        builder.HasIndex(a => new { a.PantryItemId, a.AlertType })
            .IsUnique();

        builder.HasOne(a => a.Household)
            .WithMany()
            .HasForeignKey(a => a.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.PantryItem)
            .WithMany()
            .HasForeignKey(a => a.PantryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
