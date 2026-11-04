using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Domain.Common;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Infrastructure.Identity;

namespace SmartMealPlanner.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public DbSet<Household> Households => Set<Household>();

    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();

    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();

    public DbSet<RecipeTag> RecipeTags => Set<RecipeTag>();

    public DbSet<MealPlan> MealPlans => Set<MealPlan>();

    public DbSet<MealPlanEntry> MealPlanEntries => Set<MealPlanEntry>();

    public DbSet<PantryCategory> PantryCategories => Set<PantryCategory>();

    public DbSet<PantryItem> PantryItems => Set<PantryItem>();

    public DbSet<GroceryList> GroceryLists => Set<GroceryList>();

    public DbSet<GroceryListItem> GroceryListItems => Set<GroceryListItem>();

    public DbSet<Todo> Todos => Set<Todo>();

    public DbSet<InventoryAlert> InventoryAlerts => Set<InventoryAlert>();

    public DbSet<InventoryAlertRead> InventoryAlertReads => Set<InventoryAlertRead>();

    public DbSet<InventoryAlertDismissal> InventoryAlertDismissals => Set<InventoryAlertDismissal>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("public");

        IdentityConfiguration.ConfigureIdentity(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.ApplySeedData();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;
        ApplyAuditTimestamps(utcNow);
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var utcNow = DateTimeOffset.UtcNow;
        ApplyAuditTimestamps(utcNow);
        return base.SaveChanges();
    }

    private void ApplyAuditTimestamps(DateTimeOffset utcNow)
    {
        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;

                if (entry.Entity is HouseholdMember member && member.JoinedAt == default)
                {
                    member.JoinedAt = utcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<RecipeTag>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<InventoryAlertDismissal>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;

                if (entry.Entity.DismissedAt == default)
                {
                    entry.Entity.DismissedAt = utcNow;
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<InventoryAlertRead>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;

                if (entry.Entity.ReadAt == default)
                {
                    entry.Entity.ReadAt = utcNow;
                }
            }
        }
    }
}
