using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");
        builder.ConfigureHouseholdEntity();

        builder.Property(i => i.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.DefaultUnit)
            .HasMaxLength(50);

        builder.Property(i => i.Category)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.HasIndex(i => new { i.HouseholdId, i.Name })
            .IsUnique();

        builder.HasIndex(i => new { i.HouseholdId, i.Category });

        builder.HasOne(i => i.Household)
            .WithMany()
            .HasForeignKey(i => i.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(i => i.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
