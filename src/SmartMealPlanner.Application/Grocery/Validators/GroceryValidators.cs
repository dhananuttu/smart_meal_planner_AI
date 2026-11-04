using FluentValidation;
using SmartMealPlanner.Application.Grocery.DTOs;
using SmartMealPlanner.Application.Ingredients.Constants;

namespace SmartMealPlanner.Application.Grocery.Validators;

public class CreateGroceryListRequestValidator : AbstractValidator<CreateGroceryListRequest>
{
    public CreateGroceryListRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}

public class GenerateGroceryListFromMealPlanRequestValidator
    : AbstractValidator<GenerateGroceryListFromMealPlanRequest>
{
    public GenerateGroceryListFromMealPlanRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.MealPlanId.HasValue || x.WeekStart.HasValue)
            .WithMessage("Either mealPlanId or weekStart must be provided.");

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}

public class AddGroceryItemRequestValidator : AbstractValidator<AddGroceryItemRequest>
{
    public AddGroceryItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(50)
            .Must(unit => IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Unit must be a supported unit of measure.");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}

public class AddGroceryItemsFromReceiptRequestValidator
    : AbstractValidator<AddGroceryItemsFromReceiptRequest>
{
    public AddGroceryItemsFromReceiptRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one receipt item is required.");

        RuleFor(x => x.Items)
            .Must(items => items.Count <= 100)
            .WithMessage("A receipt may contain at most 100 items.");

        RuleForEach(x => x.Items).SetValidator(new ReceiptGroceryItemRequestValidator());
    }
}

public class ReceiptGroceryItemRequestValidator : AbstractValidator<ReceiptGroceryItemRequest>
{
    public ReceiptGroceryItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Unit)
            .NotEmpty()
            .MaximumLength(50)
            .Must(unit => IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Unit must be a supported unit of measure.");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}

public class MarkGroceryItemPurchasedRequestValidator
    : AbstractValidator<MarkGroceryItemPurchasedRequest>
{
    public MarkGroceryItemPurchasedRequestValidator()
    {
        // Boolean always valid; validator exists for consistency and future rules.
    }
}
