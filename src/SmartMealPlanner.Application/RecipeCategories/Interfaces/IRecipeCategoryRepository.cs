using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.DTOs;

namespace SmartMealPlanner.Application.RecipeCategories.Interfaces;

public interface IRecipeCategoryRepository
{
    Task<PaginatedList<RecipeCategoryDto>> GetPagedAsync(
        Guid householdId,
        RecipeCategoryListQuery query,
        CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto?> GetByIdAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        Guid householdId,
        string normalizedName,
        Guid? excludeCategoryId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsInHouseholdAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto> CreateAsync(
        Guid householdId,
        Guid userId,
        string name,
        string color,
        CancellationToken cancellationToken = default);

    Task<RecipeCategoryDto?> UpdateAsync(
        Guid householdId,
        Guid categoryId,
        string name,
        string color,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid householdId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
