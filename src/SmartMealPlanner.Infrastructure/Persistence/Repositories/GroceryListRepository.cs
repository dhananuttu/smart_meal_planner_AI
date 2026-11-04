using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Grocery.Interfaces;
using SmartMealPlanner.Application.Grocery.Models;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class GroceryListRepository : IGroceryListRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GroceryListRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<GroceryListSummaryDto>> GetListsAsync(
        Guid householdId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroceryLists
            .AsNoTracking()
            .Where(l => l.HouseholdId == householdId && !l.IsArchived)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new GroceryListSummaryDto
            {
                Id = l.Id,
                Name = l.Name,
                MealPlanId = l.MealPlanId,
                IsArchived = l.IsArchived,
                ItemCount = l.Items.Count,
                PurchasedItemCount = l.Items.Count(i => i.IsChecked),
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<GroceryListDetailDto?> GetListByIdAsync(
        Guid householdId,
        Guid listId,
        CancellationToken cancellationToken = default)
    {
        var list = await _dbContext.GroceryLists
            .AsNoTracking()
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == listId && l.HouseholdId == householdId, cancellationToken);

        return list is null ? null : MapToDetailDto(list);
    }

    public Task<bool> ListBelongsToHouseholdAsync(
        Guid householdId,
        Guid listId,
        CancellationToken cancellationToken = default) =>
        _dbContext.GroceryLists.AnyAsync(
            l => l.Id == listId && l.HouseholdId == householdId,
            cancellationToken);

    public async Task<GroceryListDetailDto> CreateListAsync(
        Guid householdId,
        Guid userId,
        string name,
        Guid? mealPlanId,
        IReadOnlyList<AggregatedGroceryIngredient> initialItems,
        CancellationToken cancellationToken = default)
    {
        var list = new GroceryList
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            Name = name,
            MealPlanId = mealPlanId
        };

        foreach (var item in initialItems)
        {
            MergeItem(list, item, note: null, sourceRecipeId: item.SourceRecipeId);
        }

        _dbContext.GroceryLists.Add(list);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDetailDto(list);
    }

    public async Task<MealPlan?> ResolveMealPlanAsync(
        Guid householdId,
        Guid? mealPlanId,
        DateOnly? weekStart,
        CancellationToken cancellationToken = default)
    {
        if (mealPlanId.HasValue)
        {
            return await _dbContext.MealPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == mealPlanId.Value && p.HouseholdId == householdId,
                    cancellationToken);
        }

        if (!weekStart.HasValue)
        {
            return null;
        }

        return await _dbContext.MealPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.HouseholdId == householdId && p.WeekStartDate == weekStart.Value,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RecipeIngredientLine>> GetRecipeIngredientsForMealPlanAsync(
        Guid mealPlanId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MealPlanEntries
            .AsNoTracking()
            .Where(e => e.MealPlanId == mealPlanId && e.RecipeId != null)
            .SelectMany(e => e.Recipe!.Ingredients.Select(ri => new RecipeIngredientLine
            {
                Name = ri.Name,
                Unit = ri.Unit,
                Quantity = ri.Quantity,
                RecipeId = e.RecipeId!.Value
            }))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> RecipeExistsInHouseholdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Recipes.AnyAsync(
            r => r.HouseholdId == householdId && r.Id == recipeId && !r.IsDeleted,
            cancellationToken);

    public async Task<GroceryItemDto> AddOrMergeItemAsync(
        Guid householdId,
        Guid listId,
        AggregatedGroceryIngredient item,
        string? note,
        Guid? sourceRecipeId,
        CancellationToken cancellationToken = default)
    {
        var list = await _dbContext.GroceryLists
            .Include(l => l.Items)
            .FirstAsync(l => l.Id == listId && l.HouseholdId == householdId, cancellationToken);

        var merged = MergeItem(list, item, note, sourceRecipeId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapItem(merged);
    }

    public async Task<GroceryListDetailDto> AddOrMergeItemsAsync(
        Guid householdId,
        Guid listId,
        IReadOnlyList<AggregatedGroceryIngredient> items,
        CancellationToken cancellationToken = default)
    {
        var list = await _dbContext.GroceryLists
            .Include(l => l.Items)
            .FirstAsync(l => l.Id == listId && l.HouseholdId == householdId, cancellationToken);

        foreach (var item in items)
        {
            MergeItem(list, item, note: null, sourceRecipeId: null);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDetailDto(list);
    }

    public async Task<GroceryItemDto?> MarkItemPurchasedAsync(
        Guid householdId,
        Guid itemId,
        bool isPurchased,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.GroceryListItems
            .Include(i => i.GroceryList)
            .FirstOrDefaultAsync(
                i => i.Id == itemId && i.GroceryList.HouseholdId == householdId,
                cancellationToken);

        if (item is null)
        {
            return null;
        }

        item.IsChecked = isPurchased;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapItem(item);
    }

    public async Task<bool> DeleteItemAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.GroceryListItems
            .Include(i => i.GroceryList)
            .FirstOrDefaultAsync(
                i => i.Id == itemId && i.GroceryList.HouseholdId == householdId,
                cancellationToken);

        if (item is null)
        {
            return false;
        }

        _dbContext.GroceryListItems.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static GroceryListItem MergeItem(
        GroceryList list,
        AggregatedGroceryIngredient item,
        string? note,
        Guid? sourceRecipeId)
    {
        var normalizedName = item.Name.Trim().ToLowerInvariant();
        var normalizedUnit = item.Unit.Trim().ToLowerInvariant();

        var existing = list.Items.FirstOrDefault(i =>
            i.Name.Trim().ToLowerInvariant() == normalizedName
            && i.Unit.Trim().ToLowerInvariant() == normalizedUnit);

        if (existing is not null)
        {
            existing.Quantity += item.Quantity;
            if (!string.IsNullOrWhiteSpace(note))
            {
                existing.Note = note.Trim();
            }

            if (sourceRecipeId.HasValue)
            {
                existing.SourceRecipeId = sourceRecipeId;
            }

            return existing;
        }

        var sortOrder = list.Items.Count > 0 ? list.Items.Max(i => i.SortOrder) + 1 : 0;
        var newItem = CreateItem(list.Id, item, note, sortOrder, sourceRecipeId ?? item.SourceRecipeId);
        list.Items.Add(newItem);
        return newItem;
    }

    private static GroceryListItem CreateItem(
        Guid listId,
        AggregatedGroceryIngredient item,
        string? note,
        int sortOrder,
        Guid? sourceRecipeId = null)
    {
        return new GroceryListItem
        {
            Id = Guid.NewGuid(),
            GroceryListId = listId,
            Name = item.Name.Trim(),
            Quantity = item.Quantity,
            Unit = item.Unit.Trim().ToLowerInvariant(),
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            SourceRecipeId = sourceRecipeId ?? item.SourceRecipeId,
            SortOrder = sortOrder
        };
    }

    private static GroceryListDetailDto MapToDetailDto(GroceryList list) =>
        new()
        {
            Id = list.Id,
            Name = list.Name,
            MealPlanId = list.MealPlanId,
            IsArchived = list.IsArchived,
            Items = list.Items
                .OrderBy(i => i.SortOrder)
                .ThenBy(i => i.Name)
                .Select(MapItem)
                .ToList(),
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt
        };

    private static GroceryItemDto MapItem(GroceryListItem item) =>
        new()
        {
            Id = item.Id,
            GroceryListId = item.GroceryListId,
            Name = item.Name,
            Quantity = item.Quantity,
            Unit = item.Unit,
            IsPurchased = item.IsChecked,
            SourceRecipeId = item.SourceRecipeId,
            Note = item.Note,
            SortOrder = item.SortOrder,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
}
