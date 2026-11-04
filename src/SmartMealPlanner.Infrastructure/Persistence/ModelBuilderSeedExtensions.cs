using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Persistence.Seed;

namespace SmartMealPlanner.Infrastructure.Persistence;

internal static class ModelBuilderSeedExtensions
{
    public static void ApplySeedData(this ModelBuilder modelBuilder)
    {
        var seedTime = SeedIds.SeedTimestamp;

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = SeedIds.UserRoleId,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "seed-user-role"
            },
            new IdentityRole<Guid>
            {
                Id = SeedIds.AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "seed-admin-role"
            });

        modelBuilder.Entity<PantryCategory>().HasData(
            CreateSystemCategory(SeedIds.ProduceCategoryId, "Produce", 1, seedTime),
            CreateSystemCategory(SeedIds.DairyCategoryId, "Dairy", 2, seedTime),
            CreateSystemCategory(SeedIds.MeatCategoryId, "Meat", 3, seedTime),
            CreateSystemCategory(SeedIds.PantryStaplesCategoryId, "Pantry Staples", 4, seedTime),
            CreateSystemCategory(SeedIds.FrozenCategoryId, "Frozen", 5, seedTime),
            CreateSystemCategory(SeedIds.BeveragesCategoryId, "Beverages", 6, seedTime),
            CreateSystemCategory(SeedIds.OtherCategoryId, "Other", 99, seedTime));
    }

    private static PantryCategory CreateSystemCategory(
        Guid id,
        string name,
        int sortOrder,
        DateTimeOffset seedTime) =>
        new()
        {
            Id = id,
            Name = name,
            SortOrder = sortOrder,
            IsSystem = true,
            HouseholdId = null,
            CreatedByUserId = null,
            CreatedAt = seedTime,
            UpdatedAt = seedTime
        };
}
