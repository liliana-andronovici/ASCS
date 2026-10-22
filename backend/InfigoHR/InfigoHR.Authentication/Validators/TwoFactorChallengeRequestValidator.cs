using FluentValidation;
using InfigoHR.Authentication.Models;

namespace InfigoHR.Authentication.Validators;

public class TwoFactorChallengeRequestValidator : AbstractValidator<TwoFactorChallengeRequest>
{
    public TwoFactorChallengeRequestValidator()
    {
        RuleFor(x => x.ChallengeToken).NotEmpty().Matches("\\A[0-9A-F]{64}\\z");
        RuleFor(x => x).Must(x => TwoFactorValidationRules.HasOneProof(x.Code, x.RecoveryCode))
            .WithMessage("Supply either a six-digit code or one recovery code.");
    }
}
