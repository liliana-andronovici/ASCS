using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class TwoFactorSetupRequestValidator : AbstractValidator<TwoFactorSetupRequest>
{
    public TwoFactorSetupRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().MaximumLength(1024);
    }
}
