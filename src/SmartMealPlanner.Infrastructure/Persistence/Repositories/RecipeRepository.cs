using Microsoft.EntityFrameworkCore;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Recipes.DTOs;
using SmartMealPlanner.Application.Recipes.Interfaces;
using SmartMealPlanner.Domain.Entities;

namespace SmartMealPlanner.Infrastructure.Persistence.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RecipeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<RecipeDto>> GetPagedAsync(
        Guid householdId,
        RecipeListQuery query,
        CancellationToken cancellationToken = default)
    {
        var recipes = _dbContext.Recipes
            .AsNoTracking()
            .Include(r => r.Category)
            .Where(r => r.HouseholdId == householdId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            recipes = recipes.Where(r =>
                r.Title.ToLower().Contains(search)
                || (r.Description != null && r.Description.ToLower().Contains(search))
                || (r.Instructions != null && r.Instructions.ToLower().Contains(search)));
        }

        if (query.CategoryId.HasValue)
        {
            recipes = recipes.Where(r => r.CategoryId == query.CategoryId.Value);
        }

        var totalCount = await recipes.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await recipes
            .OrderBy(r => r.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<RecipeDto>.Create(
            items.Select(MapToDto).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<RecipeDto?> GetByIdAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _dbContext.Recipes
            .AsNoTracking()
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.HouseholdId == householdId && r.Id == recipeId, cancellationToken);

        return recipe is null ? null : MapToDto(recipe);
    }

    public async Task<RecipeDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string? description,
        string instructions,
        int? prepTime,
        int? cookTime,
        int servings,
        Guid? categoryId,
        CancellationToken cancellationToken = default)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            CreatedByUserId = userId,
            Title = name,
            Description = description,
            Instructions = instructions,
            PrepTimeMinutes = prepTime,
            CookTimeMinutes = cookTime,
            Servings = servings,
            CategoryId = categoryId
        };

        _dbContext.Recipes.Add(recipe);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (categoryId.HasValue)
        {
            await _dbContext.Entry(recipe).Reference(r => r.Category).LoadAsync(cancellationToken);
        }

        return MapToDto(recipe);
    }

    public async Task<RecipeDto?> UpdateAsync(
        Guid householdId,
        Guid recipeId,
        string name,
        string? description,
        string instructions,
        int? prepTime,
        int? cookTime,
        int servings,
        Guid? categoryId,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _dbContext.Recipes
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.HouseholdId == householdId && r.Id == recipeId, cancellationToken);

        if (recipe is null)
        {
            return null;
        }

        recipe.Title = name;
        recipe.Description = description;
        recipe.Instructions = instructions;
        recipe.PrepTimeMinutes = prepTime;
        recipe.CookTimeMinutes = cookTime;
        recipe.Servings = servings;
        recipe.CategoryId = categoryId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (categoryId.HasValue && recipe.Category is null)
        {
            await _dbContext.Entry(recipe).Reference(r => r.Category).LoadAsync(cancellationToken);
        }

        return MapToDto(recipe);
    }

    public async Task<bool> SoftDeleteAsync(
        Guid householdId,
        Guid recipeId,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _dbContext.Recipes
            .FirstOrDefaultAsync(r => r.HouseholdId == householdId && r.Id == recipeId, cancellationToken);

        if (recipe is null)
        {
            return false;
        }

        recipe.IsDeleted = true;
        recipe.DeletedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static RecipeDto MapToDto(Recipe recipe) =>
        new()
        {
            Id = recipe.Id,
            Name = recipe.Title,
            Description = recipe.Description,
            Instructions = recipe.Instructions ?? string.Empty,
            PrepTime = recipe.PrepTimeMinutes,
            CookTime = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            CategoryId = recipe.CategoryId,
            Category = recipe.Category is null
                ? null
                : new RecipeCategorySummaryDto
                {
                    Id = recipe.Category.Id,
                    Name = recipe.Category.Name,
                    Color = recipe.Category.Color
                },
            HouseholdId = recipe.HouseholdId,
            CreatedByUserId = recipe.CreatedByUserId,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt
        };
}
