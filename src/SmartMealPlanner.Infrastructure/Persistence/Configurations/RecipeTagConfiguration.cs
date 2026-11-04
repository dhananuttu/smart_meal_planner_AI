using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.ToTable("recipe_tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.Tag)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => t.RecipeId);
        builder.HasIndex(t => new { t.RecipeId, t.Tag });
        builder.HasIndex(t => t.Tag);

        builder.HasOne(t => t.Recipe)
            .WithMany(r => r.Tags)
            .HasForeignKey(t => t.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
