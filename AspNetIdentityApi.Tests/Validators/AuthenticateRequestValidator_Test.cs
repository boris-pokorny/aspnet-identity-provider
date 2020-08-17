using AspNetIdentityApi.Models;
using AspNetIdentityApi.Validators;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class AuthenticateRequestValidator_Test {
        private readonly AuthenticateRequestValidator _validator;

        public AuthenticateRequestValidator_Test () {
            _validator = new AuthenticateRequestValidator ();
        }

        [Fact]
        public void TestInvalidGrantType () {
            var request = new AuthenticateRequest {
                GrantType = ""
            };
            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ("Invalid Grant Type.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestClientCredentials () {
            var request = new AuthenticateRequest {
                GrantType = EGrantType.client_credentials.ToString (),
                Username = "a",
                Password = "a",
            };
            var result = _validator.Validate (request);

            Assert.True (result.IsValid);
        }

        [Fact]
        public void TestClientCredentials_Invalid () {
            var request = new AuthenticateRequest {
                GrantType = EGrantType.client_credentials.ToString (),
                Username = "",
                Password = "",
            };
            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (2, result.Errors.Count);
            Assert.Equal ("'Username' must not be empty.", result.Errors[0].ErrorMessage);
            Assert.Equal ("'Password' must not be empty.", result.Errors[1].ErrorMessage);
        }

        [Fact]
        public void TestRefreshToken () {
            var request = new AuthenticateRequest {
                GrantType = EGrantType.refresh_token.ToString (),
                RefreshToken = "a",
            };

            var result = _validator.Validate (request);

            Assert.True (result.IsValid);
        }

        [Fact]
        public void TestRefreshToken_Invalid () {
            var request = new AuthenticateRequest {
                GrantType = EGrantType.refresh_token.ToString (),
                RefreshToken = "",
            };
            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ("'Refresh Token' must not be empty.", result.Errors[0].ErrorMessage);
        }
    }
}