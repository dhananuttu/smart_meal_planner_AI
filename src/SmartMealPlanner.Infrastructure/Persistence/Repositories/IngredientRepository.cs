using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Application.Ingredients.Helpers;
using SmartMealPlanner.Application.Ingredients.Interfaces;
using SmartMealPlanner.Domain.Entities;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public IngredientRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<IngredientDto>> GetPagedAsync(
        Guid householdId,
        IngredientListQuery query,
        CancellationToken cancellationToken = default)
    {
        var ingredients = _dbContext.Ingredients
            .AsNoTracking()
            .Where(i => i.HouseholdId == householdId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            ingredients = ingredients.Where(i => i.Name.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category)
            && Enum.TryParse<IngredientCategory>(query.Category.Trim(), ignoreCase: true, out var category))
        {
            ingredients = ingredients.Where(i => i.Category == category);
        }

        var totalCount = await ingredients.CountAsync(cancellationToken);

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await ingredients
            .OrderBy(i => i.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<IngredientDto>.Create(
            items.Select(MapToDto).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<IngredientDto?> GetByIdAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _dbContext.Ingredients
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == ingredientId, cancellationToken);

        return ingredient is null ? null : MapToDto(ingredient);
    }

    public Task<bool> NameExistsAsync(
        Guid householdId,
        string normalizedName,
        Guid? excludeIngredientId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Ingredients
            .Where(i => i.HouseholdId == householdId && i.Name.ToLower() == normalizedName);

        if (excludeIngredientId.HasValue)
        {
            query = query.Where(i => i.Id != excludeIngredientId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<IngredientDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string? defaultUnit,
        IngredientCategory? category,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            Name = name,
            DefaultUnit = defaultUnit,
            Category = category,
            Notes = notes
        };

        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(ingredient);
    }

    public async Task<IngredientDto?> UpdateAsync(
        Guid householdId,
        Guid ingredientId,
        string name,
        string? defaultUnit,
        IngredientCategory? category,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _dbContext.Ingredients
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == ingredientId, cancellationToken);

        if (ingredient is null)
        {
            return null;
        }

        ingredient.Name = name;
        ingredient.DefaultUnit = defaultUnit;
        ingredient.Category = category;
        ingredient.Notes = notes;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(ingredient);
    }

    public async Task<bool> DeleteAsync(
        Guid householdId,
        Guid ingredientId,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _dbContext.Ingredients
            .FirstOrDefaultAsync(i => i.HouseholdId == householdId && i.Id == ingredientId, cancellationToken);

        if (ingredient is null)
        {
            return false;
        }

        _dbContext.Ingredients.Remove(ingredient);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IngredientDto MapToDto(Ingredient ingredient) =>
        new()
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            DefaultUnit = ingredient.DefaultUnit,
            Category = IngredientCategoryHelper.ToApiValue(ingredient.Category),
            Notes = ingredient.Notes,
            HouseholdId = ingredient.HouseholdId,
            CreatedByUserId = ingredient.CreatedByUserId,
            CreatedAt = ingredient.CreatedAt,
            UpdatedAt = ingredient.UpdatedAt
        };
}
