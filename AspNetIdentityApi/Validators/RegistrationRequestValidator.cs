using System.Collections.Generic;
using AspNetIdentityApi.Models;
using FluentValidation;

namespace AspNetIdentityApi.Validators {

    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest> {
        public ApplicationUserManager _userManager { get; set; }
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
                .MustAsync (async (password, cancellation) => {

                    List<string> passwordErrors = new List<string> ();

                    var validators = _userManager.PasswordValidators;

                    foreach (var validator in validators) {
                        var result = await validator.ValidateAsync (_userManager, null, password);

                        if (!result.Succeeded) {
                            foreach (var error in result.Errors) {
                                passwordErrors.Add (error.Description);
                            }
                        }
                    }
                    return passwordErrors.Count == 0;
                })
                .WithMessage ("'Password' does not comply with password rules.");
            RuleFor (x => x.ConfirmPassword)
                .NotEmpty ()
                .Must ((x, confirmedPassword) => x.Password == confirmedPassword)
                .WithMessage ("'Confirm Password' and 'Password' do not match.");
        }
    }
}