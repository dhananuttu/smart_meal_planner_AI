using FluentValidation;
using SmartMealPlanner.Application.Auth.DTOs;

namespace SmartMealPlanner.Application.Auth.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
