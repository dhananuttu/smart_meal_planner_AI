using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.ConfigureAuditableEntity();

        builder.Property(p => p.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(p => p.DietaryPreferences)
            .HasColumnType("text[]");

        builder.Property(p => p.DefaultServings)
            .HasDefaultValue(4)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_user_profiles_default_servings",
            "default_servings >= 1 AND default_servings <= 20");

        builder.Property(p => p.Timezone)
            .HasMaxLength(50)
            .HasDefaultValue("UTC")
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.HasIndex(p => p.HouseholdId);

        builder.HasOne(p => p.Household)
            .WithMany(h => h.UserProfiles)
            .HasForeignKey(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
