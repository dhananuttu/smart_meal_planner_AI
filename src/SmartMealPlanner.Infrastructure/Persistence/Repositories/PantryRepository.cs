using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.Helpers;
using SmartMealPlanner.Application.Pantry.DTOs;
using SmartMealPlanner.Application.Pantry.Interfaces;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class PantryRepository : IPantryRepository
{
    private const int ExpiringSoonDays = 7;

    private readonly ApplicationDbContext _dbContext;

    public PantryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<PantryItemDto>> GetInventoryAsync(
        Guid householdId,
        PantryInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiringSoonCutoff = today.AddDays(ExpiringSoonDays);

        var items = _dbContext.PantryItems
            .AsNoTracking()
            .Include(i => i.Ingredient)
            .Where(i => i.HouseholdId == householdId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            items = items.Where(i => i.Ingredient.Name.ToLower().Contains(search));
        }

        if (query.LowStockOnly == true)
        {
            items = items.Where(i =>
                i.LowStockThreshold != null && i.Quantity <= i.LowStockThreshold);
        }

        if (query.ExpiringSoonOnly == true)
        {
            items = items.Where(i =>
                i.ExpiryDate != null
                && i.ExpiryDate >= today
                && i.ExpiryDate <= expiringSoonCutoff);
        }

        var totalCount = await items.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var results = await items
            .OrderBy(i => i.Ingredient.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<PantryItemDto>.Create(
            results.Select(i => MapToDto(i, today, expiringSoonCutoff)).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<PantryItemDto?> GetByIdAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiringSoonCutoff = today.AddDays(ExpiringSoonDays);

        var item = await _dbContext.PantryItems
            .AsNoTracking()
            .Include(i => i.Ingredient)
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == itemId, cancellationToken);

        return item is null ? null : MapToDto(item, today, expiringSoonCutoff);
    }

    public Task<bool> IngredientExistsInHouseholdAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Ingredients.AnyAsync(
            i => i.HouseholdId == householdId && i.Id == ingredientId,
            cancellationToken);

    public Task<bool> IngredientAlreadyInPantryAsync(
        Guid householdId,
        Guid ingredientId,
        Guid? excludeItemId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PantryItems
            .Where(i => i.HouseholdId == householdId && i.IngredientId == ingredientId);

        if (excludeItemId.HasValue)
        {
            query = query.Where(i => i.Id != excludeItemId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<PantryItemDto> CreateAsync(
        Guid householdId,
        Guid userId,
        Guid ingredientId,
        decimal quantity,
        string unit,
        DateOnly? expirationDate,
        decimal? lowStockThreshold,
        CancellationToken cancellationToken = default)
    {
        var item = new PantryItem
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            IngredientId = ingredientId,
            Quantity = quantity,
            Unit = unit,
            ExpiryDate = expirationDate,
            LowStockThreshold = lowStockThreshold
        };

        _dbContext.PantryItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(item).Reference(i => i.Ingredient).LoadAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return MapToDto(item, today, today.AddDays(ExpiringSoonDays));
    }

    public async Task<PantryItemDto?> UpdateAsync(
        Guid householdId,
        Guid itemId,
        decimal quantity,
        string unit,
        DateOnly? expirationDate,
        decimal? lowStockThreshold,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.PantryItems
            .Include(i => i.Ingredient)
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == itemId, cancellationToken);

        if (item is null)
        {
            return null;
        }

        item.Quantity = quantity;
        item.Unit = unit;
        item.ExpiryDate = expirationDate;
        item.LowStockThreshold = lowStockThreshold;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return MapToDto(item, today, today.AddDays(ExpiringSoonDays));
    }

    public async Task<bool> DeleteAsync(
        Guid householdId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.PantryItems
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == itemId, cancellationToken);

        if (item is null)
        {
            return false;
        }

        _dbContext.PantryItems.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PantryItemDto MapToDto(
        PantryItem item,
        DateOnly today,
        DateOnly expiringSoonCutoff) =>
        new()
        {
            Id = item.Id,
            IngredientId = item.IngredientId,
            Ingredient = new PantryIngredientSummaryDto
            {
                Id = item.Ingredient.Id,
                Name = item.Ingredient.Name,
                Category = IngredientCategoryHelper.ToApiValue(item.Ingredient.Category)
            },
            Quantity = item.Quantity,
            Unit = item.Unit,
            ExpirationDate = item.ExpiryDate,
            LowStockThreshold = item.LowStockThreshold,
            IsLowStock = item.LowStockThreshold.HasValue && item.Quantity <= item.LowStockThreshold,
            IsExpired = item.ExpiryDate.HasValue && item.ExpiryDate < today,
            IsExpiringSoon = item.ExpiryDate.HasValue
                && item.ExpiryDate >= today
                && item.ExpiryDate <= expiringSoonCutoff,
            HouseholdId = item.HouseholdId,
            CreatedByUserId = item.CreatedByUserId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
}
