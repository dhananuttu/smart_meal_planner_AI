using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class GroceryListItemConfiguration : IEntityTypeConfiguration<GroceryListItem>
{
    public void Configure(EntityTypeBuilder<GroceryListItem> builder)
    {
        builder.ToTable("grocery_list_items");
        builder.ConfigureAuditableEntity();

        builder.Property(i => i.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasPrecision(10, 2)
            .HasDefaultValue(1m)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_grocery_list_items_quantity",
            "quantity > 0");

        builder.Property(i => i.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.IsChecked)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(i => i.Note)
            .HasMaxLength(500);

        builder.Property(i => i.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(i => i.GroceryListId);
        builder.HasIndex(i => new { i.GroceryListId, i.IsChecked });
        builder.HasIndex(i => i.SourceRecipeId);

        builder.HasOne(i => i.GroceryList)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.GroceryListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.SourceRecipe)
            .WithMany()
            .HasForeignKey(i => i.SourceRecipeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
