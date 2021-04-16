using AspNetIdentityApi.Models;
using FluentValidation;

namespace AspNetIdentityApi.Validators {

    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest> {
        private ApplicationUserManager _userManager { get; set; }
        public RegistrationRequestValidator (ApplicationUserManager userManager) {

            _userManager = userManager;

            RuleFor (x => x.FirstName).NotEmpty ();
            RuleFor (x => x.LastName).NotEmpty ();
            RuleFor (x => x.UserName)
                .Cascade (CascadeMode.Stop)
                .NotEmpty ()
                .MustAsync (async (name, cancellation) => {
                    var found = await _userManager.FindByNameAsync (name);
                    return found is null;
                })
                .WithMessage (x => $"User Name {x.UserName} already taken.");
            RuleFor (x => x.Email)
                .Cascade (CascadeMode.Stop)
                .NotEmpty ()
                .MustAsync (async (email, cancellation) => {
                    var found = await _userManager.FindByEmailAsync (email);
                    return found is null;
                })
                .WithMessage (x => $"An account with email {x.Email} already exists.");
            RuleFor (x => x.Password)
                .Cascade (CascadeMode.Stop)
                .NotEmpty ()
                .CustomAsync (async (password, context, cancellation) => {

                    var validators = _userManager.PasswordValidators;

                    foreach (var validator in validators) {
                        var result = await validator.ValidateAsync (_userManager, null, password);

                        if (!result.Succeeded) {
                            foreach (var error in result.Errors) {
                                context.AddFailure (error.Description);
                            }
                        }
                    }
                });
            RuleFor (x => x.ConfirmPassword)
                .NotEmpty ()
                .Must ((x, confirmedPassword) => x.Password == confirmedPassword)
                .WithMessage ("'Confirm Password' and 'Password' do not match.");
        }
    }
}