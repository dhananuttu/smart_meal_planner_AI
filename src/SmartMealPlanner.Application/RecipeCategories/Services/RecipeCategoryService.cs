using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.RecipeCategories.DTOs;
using SmartMealPlanner.Application.RecipeCategories.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.RecipeCategories.Services;

public class RecipeCategoryService : IRecipeCategoryService
{
    private readonly IRecipeCategoryRepository _repository;
    private readonly IHouseholdContext _householdContext;

    public RecipeCategoryService(
        IRecipeCategoryRepository repository,
        IHouseholdContext householdContext)
    {
        _repository = repository;
        _householdContext = householdContext;
    }

    public async Task<PaginatedList<RecipeCategoryDto>> GetAllAsync(
        RecipeCategoryListQuery query,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _repository.GetPagedAsync(_householdContext.HouseholdId, query, cancellationToken);
    }

    public async Task<RecipeCategoryDto> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _repository.GetByIdAsync(_householdContext.HouseholdId, categoryId, cancellationToken)
            ?? throw new NotFoundException("Recipe category", categoryId);
    }

    public async Task<RecipeCategoryDto> CreateAsync(
        CreateRecipeCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var name = request.Name.Trim();
        if (await _repository.NameExistsAsync(
            _householdContext.HouseholdId,
            name.ToLowerInvariant(),
            cancellationToken: cancellationToken))
        {
            throw new ConflictException($"A recipe category named '{name}' already exists.");
        }

        return await _repository.CreateAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            name,
            request.Color.Trim().ToUpperInvariant(),
            cancellationToken);
    }

    public async Task<RecipeCategoryDto> UpdateAsync(
        Guid categoryId,
        UpdateRecipeCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var name = request.Name.Trim();
        if (await _repository.NameExistsAsync(
            _householdContext.HouseholdId,
            name.ToLowerInvariant(),
            categoryId,
            cancellationToken))
        {
            throw new ConflictException($"A recipe category named '{name}' already exists.");
        }

        return await _repository.UpdateAsync(
            _householdContext.HouseholdId,
            categoryId,
            name,
            request.Color.Trim().ToUpperInvariant(),
            cancellationToken)
            ?? throw new NotFoundException("Recipe category", categoryId);
    }

    public async Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var deleted = await _repository.DeleteAsync(_householdContext.HouseholdId, categoryId, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Recipe category", categoryId);
        }
    }
}
