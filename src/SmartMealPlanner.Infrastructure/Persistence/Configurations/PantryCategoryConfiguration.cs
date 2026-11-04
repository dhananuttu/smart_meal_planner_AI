using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class PantryCategoryConfiguration : IEntityTypeConfiguration<PantryCategory>
{
    public void Configure(EntityTypeBuilder<PantryCategory> builder)
    {
        builder.ToTable("pantry_categories");
        builder.ConfigureAuditableEntity();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.IsSystem)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(c => c.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(c => new { c.HouseholdId, c.IsSystem });

        builder.HasIndex(c => new { c.HouseholdId, c.Name })
            .IsUnique()
            .HasFilter("household_id IS NOT NULL");

        builder.HasOne(c => c.Household)
            .WithMany()
            .HasForeignKey(c => c.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
