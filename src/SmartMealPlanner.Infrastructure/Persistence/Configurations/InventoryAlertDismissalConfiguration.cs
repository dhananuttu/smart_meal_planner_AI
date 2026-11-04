using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class InventoryAlertDismissalConfiguration : IEntityTypeConfiguration<InventoryAlertDismissal>
{
    public void Configure(EntityTypeBuilder<InventoryAlertDismissal> builder)
    {
        builder.ToTable("inventory_alert_dismissals");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(d => d.AlertType)
            .HasConversion(
                type => ValueConverters.AlertTypeToString(type),
                value => ValueConverters.AlertTypeFromString(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_inventory_alert_dismissals_alert_type",
            "alert_type IN ('low_stock', 'out_of_stock', 'expiring_soon', 'expired')"));

        builder.Property(d => d.DismissedAt)
            .IsRequired();

        builder.Property(d => d.ConditionSnapshot)
            .HasMaxLength(100);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasIndex(d => new { d.UserId, d.PantryItemId, d.AlertType })
            .IsUnique();

        builder.HasIndex(d => new { d.UserId, d.DismissedAt });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.PantryItem)
            .WithMany()
            .HasForeignKey(d => d.PantryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
