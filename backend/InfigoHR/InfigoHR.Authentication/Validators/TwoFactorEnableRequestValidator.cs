using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class TwoFactorEnableRequestValidator : AbstractValidator<TwoFactorEnableRequest>
{
    public TwoFactorEnableRequestValidator()
    {
        RuleFor(x => x.SetupId).NotEmpty();
        RuleFor(x => x.Code).Must(TwoFactorValidationRules.IsCode)
            .WithMessage("Code must contain exactly six digits.");
    }
}
