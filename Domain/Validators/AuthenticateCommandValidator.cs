using Domain.Commands;
using Domain.Model;
using FluentValidation;

namespace Domain.Validators;

public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateCommandValidator()
    {
        RuleFor(x => x.GrantType)
            .NotEmpty()
            .Must(v => new [] { GrantType.Password, GrantType.RefreshToken}.Contains(v));
        RuleFor(x => x.UserName)
            .NotEmpty()
            .When(x => x.GrantType == GrantType.Password);
        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => x.GrantType == GrantType.Password);
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .When(x => x.GrantType == GrantType.RefreshToken);
    }
}