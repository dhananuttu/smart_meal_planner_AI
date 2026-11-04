using FluentValidation;
using SmartMealPlanner.Application.Ingredients.Constants;
using SmartMealPlanner.Application.Pantry.DTOs;

namespace SmartMealPlanner.Application.Pantry.Validators;

public class CreatePantryItemRequestValidator : AbstractValidator<CreatePantryItemRequest>
{
    public CreatePantryItemRequestValidator()
    {
        RuleFor(x => x.IngredientId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(50)
            .Must(unit => IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Unit must be a supported unit of measure.");

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LowStockThreshold.HasValue);
    }
}

public class UpdatePantryItemRequestValidator : AbstractValidator<UpdatePantryItemRequest>
{
    public UpdatePantryItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(50)
            .Must(unit => IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Unit must be a supported unit of measure.");

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LowStockThreshold.HasValue);
    }
}

public class PantryInventoryQueryValidator : AbstractValidator<PantryInventoryQuery>
{
    public PantryInventoryQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));
    }
}
