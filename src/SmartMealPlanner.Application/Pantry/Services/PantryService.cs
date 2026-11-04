using SmartMealPlanner.Application.Common.Interfaces;
using SmartMealPlanner.Application.Common.Models;
using SmartMealPlanner.Application.Pantry.DTOs;
using SmartMealPlanner.Application.Pantry.Interfaces;
using SmartMealPlanner.Domain.Exceptions;

namespace SmartMealPlanner.Application.Pantry.Services;

public class PantryService : IPantryService
{
    private readonly IPantryRepository _pantryRepository;
    private readonly IHouseholdContext _householdContext;

    public PantryService(
        IPantryRepository pantryRepository,
        IHouseholdContext householdContext)
    {
        _pantryRepository = pantryRepository;
        _householdContext = householdContext;
    }

    public async Task<PaginatedList<PantryItemDto>> GetInventoryAsync(
        PantryInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _pantryRepository.GetInventoryAsync(
            _householdContext.HouseholdId,
            query,
            cancellationToken);
    }

    public async Task<PantryItemDto> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        return await _pantryRepository.GetByIdAsync(_householdContext.HouseholdId, itemId, cancellationToken)
            ?? throw new NotFoundException("Pantry item", itemId);
    }

    public async Task<PantryItemDto> AddItemAsync(
        CreatePantryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);
        await ValidateIngredientAsync(request.IngredientId, cancellationToken);

        if (await _pantryRepository.IngredientAlreadyInPantryAsync(
            _householdContext.HouseholdId,
            request.IngredientId,
            cancellationToken: cancellationToken))
        {
            throw new ConflictException("This ingredient is already tracked in the pantry. Update the existing item instead.");
        }

        return await _pantryRepository.CreateAsync(
            _householdContext.HouseholdId,
            _householdContext.UserId,
            request.IngredientId,
            request.Quantity,
            request.Unit.Trim().ToLowerInvariant(),
            request.ExpirationDate,
            request.LowStockThreshold,
            cancellationToken);
    }

    public async Task<PantryItemDto> UpdateItemAsync(
        Guid itemId,
        UpdatePantryItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        return await _pantryRepository.UpdateAsync(
            _householdContext.HouseholdId,
            itemId,
            request.Quantity,
            request.Unit.Trim().ToLowerInvariant(),
            request.ExpirationDate,
            request.LowStockThreshold,
            cancellationToken)
            ?? throw new NotFoundException("Pantry item", itemId);
    }

    public async Task DeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        await _householdContext.EnsureMemberAsync(cancellationToken);

        var deleted = await _pantryRepository.DeleteAsync(
            _householdContext.HouseholdId,
            itemId,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Pantry item", itemId);
        }
    }

    private async Task ValidateIngredientAsync(Guid ingredientId, CancellationToken cancellationToken)
    {
        var exists = await _pantryRepository.IngredientExistsInHouseholdAsync(
            _householdContext.HouseholdId,
            ingredientId,
            cancellationToken);

        if (!exists)
        {
            throw new NotFoundException("Ingredient", ingredientId);
        }
    }
}
