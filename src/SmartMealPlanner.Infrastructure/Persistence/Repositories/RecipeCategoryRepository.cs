using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.DTOs;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class RecipeCategoryRepository : IRecipeCategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RecipeCategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<RecipeCategoryDto>> GetPagedAsync(
        Guid householdId,
        RecipeCategoryListQuery query,
        CancellationToken cancellationToken = default)
    {
        var categories = _dbContext.RecipeCategories
            .AsNoTracking()
            .Where(c => c.HouseholdId == householdId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            categories = categories.Where(c => c.Name.ToLower().Contains(search));
        }

        var totalCount = await categories.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await categories
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<RecipeCategoryDto>.Create(
            items.Select(MapToDto).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<RecipeCategoryDto?> GetByIdAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.RecipeCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.HouseholdId == householdId && c.Id == categoryId, cancellationToken);

        return category is null ? null : MapToDto(category);
    }

    public Task<bool> NameExistsAsync(
        Guid householdId,
        string normalizedName,
        Guid? excludeCategoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.RecipeCategories
            .Where(c => c.HouseholdId == householdId && c.Name.ToLower() == normalizedName);

        if (excludeCategoryId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCategoryId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsInHouseholdAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default) =>
        _dbContext.RecipeCategories
            .AnyAsync(c => c.HouseholdId == householdId && c.Id == categoryId, cancellationToken);

    public async Task<RecipeCategoryDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string color,
        CancellationToken cancellationToken = default)
    {
        var category = new RecipeCategory
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            Name = name,
            Color = color
        };

        _dbContext.RecipeCategories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<RecipeCategoryDto?> UpdateAsync(
        Guid householdId,
        Guid categoryId,
        string name,
        string color,
        CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.RecipeCategories
            .FirstOrDefaultAsync(c => c.HouseholdId == householdId && c.Id == categoryId, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = name;
        category.Color = color;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.RecipeCategories
            .FirstOrDefaultAsync(c => c.HouseholdId == householdId && c.Id == categoryId, cancellationToken);

        if (category is null)
        {
            return false;
        }

        _dbContext.RecipeCategories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RecipeCategoryDto MapToDto(RecipeCategory category) =>
        new()
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            HouseholdId = category.HouseholdId,
            CreatedByUserId = category.CreatedByUserId,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
}
