using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("recipe_steps");
        builder.ConfigureAuditableEntity();

        builder.Property(s => s.StepNumber)
            .IsRequired();

        builder.HasCheckConstraint(
            "ck_recipe_steps_step_number",
            "step_number >= 1");

        builder.Property(s => s.Instruction)
            .IsRequired();

        builder.HasIndex(s => s.RecipeId);
        builder.HasIndex(s => new { s.RecipeId, s.StepNumber })
            .IsUnique();

        builder.HasOne(s => s.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(s => s.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
