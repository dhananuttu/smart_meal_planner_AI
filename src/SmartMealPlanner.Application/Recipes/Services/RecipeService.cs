using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Application.Recipes.DTOs;
using SmartMealPlanner.Application.Recipes.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Recipes.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeCategoryRepository _categoryRepository;
    private readonly IHouseholdContext _householdContext;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IRecipeCategoryRepository categoryRepository,
        IHouseholdContext householdContext)
    {
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _householdContext = householdContext;
    }

    public async Task<PaginatedList<RecipeDto>> GetAllAsync(
        RecipeListQuery query,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _recipeRepository.GetPagedAsync(
            _householdContext.HouseholdId,
            query,
            cancellationToken);
    }

    public async Task<RecipeDto> GetByIdAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _recipeRepository.GetByIdAsync(_householdContext.HouseholdId, recipeId, cancellationToken)
            ?? throw new NotFoundException("Recipe", recipeId);
    }

    public async Task<RecipeDto> CreateAsync(
        CreateRecipeRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        await ValidateCategoryAsync(request.CategoryId, cancellationToken);

        return await _recipeRepository.CreateAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            request.Instructions.Trim(),
            request.PrepTime,
            request.CookTime,
            request.Servings,
            request.CategoryId,
            cancellationToken);
    }

    public async Task<RecipeDto> UpdateAsync(
        Guid recipeId,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        await ValidateCategoryAsync(request.CategoryId, cancellationToken);

        return await _recipeRepository.UpdateAsync(
            _householdContext.HouseholdId,
            recipeId,
            request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            request.Instructions.Trim(),
            request.PrepTime,
            request.CookTime,
            request.Servings,
            request.CategoryId,
            cancellationToken)
            ?? throw new NotFoundException("Recipe", recipeId);
    }

    public async Task DeleteAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var deleted = await _recipeRepository.SoftDeleteAsync(
            _householdContext.HouseholdId,
            recipeId,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Recipe", recipeId);
        }
    }

    private async Task ValidateCategoryAsync(Guid? categoryId, CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
        {
            return;
        }

        var exists = await _categoryRepository.ExistsInHouseholdAsync(
            _householdContext.HouseholdId,
            categoryId.Value,
            cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Recipe category", categoryId.Value);
        }
    }
}
