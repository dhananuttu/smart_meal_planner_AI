using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class InventoryAlertReadConfiguration : IEntityTypeConfiguration<InventoryAlertRead>
{
    public void Configure(EntityTypeBuilder<InventoryAlertRead> builder)
    {
        builder.ToTable("inventory_alert_reads");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.ReadAt)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => new { r.UserId, r.AlertId })
            .IsUnique();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Alert)
            .WithMany()
            .HasForeignKey(r => r.AlertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
