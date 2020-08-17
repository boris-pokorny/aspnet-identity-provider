using System;
using AspNetIdentityApi.Models;
using FluentValidation;

namespace AspNetIdentityApi.Validators {

    public class AuthenticateRequestValidator : AbstractValidator<AuthenticateRequest> {
        public AuthenticateRequestValidator () {
            EGrantType gt;

            RuleFor (x => Enum.TryParse (x.GrantType, out gt)).Equal (true).WithMessage ("Invalid Grant Type.");
            RuleFor (x => x.Username).NotEmpty ().When (x => x.GrantType == EGrantType.client_credentials.ToString ());
            RuleFor (x => x.Password).NotEmpty ().When (x => x.GrantType == EGrantType.client_credentials.ToString ());
            RuleFor (x => x.RefreshToken).NotEmpty ().When (x => x.GrantType == EGrantType.refresh_token.ToString ());
        }
    }
}