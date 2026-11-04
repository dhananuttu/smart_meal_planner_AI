using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMealPlanner.Domain.Common;

namespace SmartMealPlanner.Infrastructure.Persistence.Configurations;

internal static class EntityConfigurationExtensions
{
    public static void ConfigureAuditableEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }

    public static void ConfigureHouseholdEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : HouseholdEntity
    {
        builder.ConfigureAuditableEntity();

        builder.Property(e => e.HouseholdId)
            .IsRequired();

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();
    }

    public static void ConfigureUserOwnedEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : UserOwnedEntity
    {
        builder.ConfigureAuditableEntity();

        builder.Property(e => e.UserId)
            .IsRequired();
    }
}
