using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.DTOs;

namespace SmartMealPlanner.Application.RecipeCategories.Interfaces;

public interface IRecipeCategoryService
{
    Task<PaginatedList<RecipeCategoryDto>> GetAllAsync(
        RecipeCategoryListQuery query,
        CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto> CreateAsync(
        CreateRecipeCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto> UpdateAsync(
        Guid categoryId,
        UpdateRecipeCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
