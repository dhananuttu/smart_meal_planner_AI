using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class HouseholdMemberConfiguration : IEntityTypeConfiguration<HouseholdMember>
{
    public void Configure(EntityTypeBuilder<HouseholdMember> builder)
    {
        builder.ToTable("household_members");
        builder.ConfigureAuditableEntity();

        builder.Property(m => m.Role)
            .HasConversion(
                role => ValueConverters.HouseholdRoleToString(role),
                value => ValueConverters.HouseholdRoleFromString(value))
            .HasMaxLength(20)
            .HasDefaultValue(HouseholdRole.Member)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_household_members_role",
            "role IN ('owner', 'member')");

        builder.Property(m => m.DisplayLabel)
            .HasMaxLength(50);

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        builder.HasIndex(m => new { m.HouseholdId, m.UserId })
            .IsUnique();

        builder.HasIndex(m => m.UserId);

        builder.HasIndex(m => m.HouseholdId)
            .IsUnique()
            .HasFilter("role = 'owner'");

        builder.HasOne(m => m.Household)
            .WithMany(h => h.Members)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
