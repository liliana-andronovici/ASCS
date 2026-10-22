using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class TwoFactorManagementRequestValidator : AbstractValidator<TwoFactorManagementRequest>
{
    public TwoFactorManagementRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty().MaximumLength(1024);
        RuleFor(x => x).Must(x =>
                TwoFactorValidationRules.HasOneProof(x.Code, x.RecoveryCode))
            .WithMessage("Supply either a six-digit code or one recovery code.");
    }
}
