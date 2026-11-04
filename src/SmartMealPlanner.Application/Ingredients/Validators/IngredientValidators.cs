using FluentValidation;
using SmartMealPlanner.Application.Ingredients.Constants;
using SmartMealPlanner.Application.Ingredients.DTOs;
using SmartMealPlanner.Domain.Enums;

namespace SmartMealPlanner.Application.Ingredients.Validators;

public class CreateIngredientRequestValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DefaultUnit)
            .MaximumLength(50)
            .Must(unit => string.IsNullOrWhiteSpace(unit) || IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Default unit must be a supported unit of measure.")
            .When(x => !string.IsNullOrWhiteSpace(x.DefaultUnit));

        RuleFor(x => x.Category)
            .Must(IsValidCategory)
            .WithMessage("Category must be a valid ingredient category.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }

    private static bool IsValidCategory(string? category) =>
        string.IsNullOrWhiteSpace(category)
        || Enum.TryParse<IngredientCategory>(category.Trim(), ignoreCase: true, out _);
}

public class UpdateIngredientRequestValidator : AbstractValidator<UpdateIngredientRequest>
{
    public UpdateIngredientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DefaultUnit)
            .MaximumLength(50)
            .Must(unit => string.IsNullOrWhiteSpace(unit) || IngredientUnits.AllowedValues.Contains(unit.Trim()))
            .WithMessage("Default unit must be a supported unit of measure.")
            .When(x => !string.IsNullOrWhiteSpace(x.DefaultUnit));

        RuleFor(x => x.Category)
            .Must(IsValidCategory)
            .WithMessage("Category must be a valid ingredient category.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }

    private static bool IsValidCategory(string? category) =>
        string.IsNullOrWhiteSpace(category)
        || Enum.TryParse<IngredientCategory>(category.Trim(), ignoreCase: true, out _);
}

public class IngredientListQueryValidator : AbstractValidator<IngredientListQuery>
{
    public IngredientListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Category)
            .Must(category => string.IsNullOrWhiteSpace(category)
                || Enum.TryParse<IngredientCategory>(category.Trim(), ignoreCase: true, out _))
            .WithMessage("Category must be a valid ingredient category.")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));
    }
}
