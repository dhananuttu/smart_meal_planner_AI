using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes");
        builder.ConfigureHouseholdEntity();

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Servings)
            .HasDefaultValue(4)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_recipes_servings",
            "servings >= 1 AND servings <= 50");

        builder.HasCheckConstraint(
            "ck_recipes_prep_time_minutes",
            "prep_time_minutes IS NULL OR (prep_time_minutes >= 0 AND prep_time_minutes <= 1440)");

        builder.HasCheckConstraint(
            "ck_recipes_cook_time_minutes",
            "cook_time_minutes IS NULL OR (cook_time_minutes >= 0 AND cook_time_minutes <= 1440)");

        builder.Property(r => r.Instructions)
            .HasColumnType("text");

        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(r => new { r.HouseholdId, r.IsDeleted });
        builder.HasIndex(r => new { r.HouseholdId, r.CategoryId });
        builder.HasIndex(r => new { r.HouseholdId, r.Title });
        builder.HasIndex(r => r.CreatedByUserId);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne(r => r.Household)
            .WithMany()
            .HasForeignKey(r => r.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
