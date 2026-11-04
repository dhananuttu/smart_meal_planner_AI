using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Application.Ingredients.Helpers;
using SmartMealPlanner.Application.Ingredients.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Ingredients.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IHouseholdContext _householdContext;

    public IngredientService(
        IIngredientRepository ingredientRepository,
        IHouseholdContext householdContext)
    {
        _ingredientRepository = ingredientRepository;
        _householdContext = householdContext;
    }

    public async Task<PaginatedList<IngredientDto>> GetAllAsync(
        IngredientListQuery query,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _ingredientRepository.GetPagedAsync(
            _householdContext.HouseholdId,
            query,
            cancellationToken);
    }

    public async Task<IngredientDto> GetByIdAsync(Guid ingredientId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _ingredientRepository.GetByIdAsync(
            _householdContext.HouseholdId,
            ingredientId,
            cancellationToken)
            ?? throw new NotFoundException("Ingredient", ingredientId);
    }

    public async Task<IngredientDto> CreateAsync(
        CreateIngredientRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var name = request.Name.Trim();
        var normalizedName = name.ToLowerInvariant();

        if (await _ingredientRepository.NameExistsAsync(
            _householdContext.HouseholdId,
            normalizedName,
            cancellationToken: cancellationToken))
        {
            throw new ConflictException($"An ingredient named '{name}' already exists in this household.");
        }

        return await _ingredientRepository.CreateAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            name,
            NormalizeUnit(request.DefaultUnit),
            IngredientCategoryHelper.Parse(request.Category),
            string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            cancellationToken);
    }

    public async Task<IngredientDto> UpdateAsync(
        Guid ingredientId,
        UpdateIngredientRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var name = request.Name.Trim();
        var normalizedName = name.ToLowerInvariant();

        if (await _ingredientRepository.NameExistsAsync(
            _householdContext.HouseholdId,
            normalizedName,
            ingredientId,
            cancellationToken))
        {
            throw new ConflictException($"An ingredient named '{name}' already exists in this household.");
        }

        return await _ingredientRepository.UpdateAsync(
            _householdContext.HouseholdId,
            ingredientId,
            name,
            NormalizeUnit(request.DefaultUnit),
            IngredientCategoryHelper.Parse(request.Category),
            string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            cancellationToken)
            ?? throw new NotFoundException("Ingredient", ingredientId);
    }

    public async Task DeleteAsync(Guid ingredientId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var deleted = await _ingredientRepository.DeleteAsync(
            _householdContext.HouseholdId,
            ingredientId,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Ingredient", ingredientId);
        }
    }

    private static string? NormalizeUnit(string? unit) =>
        string.IsNullOrWhiteSpace(unit) ? null : unit.Trim().ToLowerInvariant();
}
