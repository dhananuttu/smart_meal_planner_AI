using System.Text.RegularExpressions;
using FluentValidation;
using SmartMealPlanner.Application.RecipeCategories.DTOs;

namespace SmartMealPlanner.Application.RecipeCategories.Validators;

public partial class RecipeCategoryValidators
{
    [GeneratedRegex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled)]
    private static partial Regex HexColorRegex();

    public static bool IsValidColor(string color) =>
        HexColorRegex().IsMatch(color.Trim());
}

public class CreateRecipeCategoryRequestValidator : AbstractValidator<CreateRecipeCategoryRequest>
{
    public CreateRecipeCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(7)
            .Must(RecipeCategoryValidators.IsValidColor)
            .WithMessage("Color must be a valid hex value (e.g. #4CAF50 or #FFF).");
    }
}

public class UpdateRecipeCategoryRequestValidator : AbstractValidator<UpdateRecipeCategoryRequest>
{
    public UpdateRecipeCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .NotEmpty()
            .MaximumLength(7)
            .Must(RecipeCategoryValidators.IsValidColor)
            .WithMessage("Color must be a valid hex value (e.g. #4CAF50 or #FFF).");
    }
}

public class RecipeCategoryListQueryValidator : AbstractValidator<RecipeCategoryListQuery>
{
    public RecipeCategoryListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Search));
    }
}
