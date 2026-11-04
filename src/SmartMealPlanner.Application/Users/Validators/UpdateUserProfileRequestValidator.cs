using FluentValidation;
using SmartMealPlanner.Application.Users.Constants;
using SmartMealPlanner.Application.Users.DTOs;

namespace SmartMealPlanner.Application.Users.Validators;

public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Avatar URL must be a valid absolute URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl));

        RuleFor(x => x.DefaultServings)
            .InclusiveBetween(1, 20);

        RuleFor(x => x.Timezone)
            .NotEmpty()
            .MaximumLength(50)
            .Must(IsValidTimezone)
            .WithMessage("Timezone must be a valid IANA timezone identifier.");

        RuleFor(x => x.DietaryPreferences)
            .Must(prefs => prefs.Count <= 10)
            .WithMessage("A maximum of 10 dietary preferences is allowed.");

        RuleForEach(x => x.DietaryPreferences)
            .Must(pref => DietaryPreferences.AllowedValues.Contains(pref.Trim()))
            .WithMessage("'{PropertyValue}' is not a supported dietary preference.");
    }

    private static bool IsValidTimezone(string timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone))
        {
            return false;
        }

        try
        {
            _ = TimeZoneInfo.FindSystemTimeZoneById(timezone.Trim());
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
