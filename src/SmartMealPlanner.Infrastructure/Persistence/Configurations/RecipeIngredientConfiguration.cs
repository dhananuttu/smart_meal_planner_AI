using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");
        builder.ConfigureAuditableEntity();

        builder.Property(i => i.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_recipe_ingredients_quantity",
            "quantity >= 0");

        builder.Property(i => i.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(i => i.RecipeId);
        builder.HasIndex(i => new { i.RecipeId, i.SortOrder });

        builder.HasOne(i => i.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(i => i.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
