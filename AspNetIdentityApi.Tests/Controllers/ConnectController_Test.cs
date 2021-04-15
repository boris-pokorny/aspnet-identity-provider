using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetIdentityApi.Controllers;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public interface IError {
        string message { get; set; }
    }

    public class MockTokenService : ITokenService {
        public TokenResponse Response = new TokenResponse { };
        public string GenerateJwtToken (ApplicationUser user) {
            return "";
        }
        public string GenerateRefreshToken () {
            return "";
        }
        public TokenResponse CreateResponse (ApplicationUser user, string accessToken, string refreshToken) {
            return Response;
        }
        public bool IsRefreshExpired (string token) {
            return token == "expired";
        }
    }

    public class ConnectController_Test {
        private readonly ConnectController _connectController;
        private readonly Mock<IUserService> _userService;
        private readonly ApplicationUser _user = new ApplicationUser { Id = "0000" };
        private readonly ApplicationUser _userExpired = new ApplicationUser { Id = "0001" };
        private readonly AuthenticateRequest _request = new AuthenticateRequest {
            GrantType = EGrantType.client_credentials.ToString (),
        };
        private readonly AuthenticateRequest _requestFail = new AuthenticateRequest {
            GrantType = EGrantType.client_credentials.ToString (),
        };
        private readonly AuthenticateRequest _requestRefresh = new AuthenticateRequest {
            GrantType = EGrantType.refresh_token.ToString (),
        };
        private readonly AuthenticateRequest _requestRefreshFail = new AuthenticateRequest {
            GrantType = EGrantType.refresh_token.ToString (),
            RefreshToken = "token",
        };
        private readonly AuthenticateRequest _requestRefreshExpired = new AuthenticateRequest {
            GrantType = EGrantType.refresh_token.ToString (),
            RefreshToken = "expired",
        };

        public ConnectController_Test () {
            var mockTokenService = new MockTokenService ();

            _userService = new Mock<IUserService> ();
            _userService.Setup (a => a.Authenticate (_request)).Returns (Task.FromResult<ApplicationUser> (_user));
            _userService.Setup (a => a.Authenticate (_requestFail)).Returns (Task.FromResult<ApplicationUser> (null));
            _userService.Setup (a => a.Authenticate (_requestRefresh)).Returns (Task.FromResult<ApplicationUser> (_user));
            _userService.Setup (a => a.Authenticate (_requestRefreshExpired)).Returns (Task.FromResult<ApplicationUser> (_userExpired));
            _userService.Setup (a => a.SetTokens (_user, "", ""));
            _userService.Setup (a => a.RemoveTokens (_user));
            _userService.Setup (a => a.HasValidRefreshToken (_requestRefresh.RefreshToken)).Returns (_user);
            _userService.Setup (a => a.HasValidRefreshToken (_requestRefreshFail.RefreshToken)).Returns ((ApplicationUser) null);
            _userService.Setup (a => a.HasValidRefreshToken (_requestRefreshExpired.RefreshToken)).Returns (_userExpired);
            _userService.Setup (a => a.GetAccessToken (_userExpired)).Returns (Task.FromResult ("expired"));

            _connectController = new ConnectController (null, _userService.Object, mockTokenService);
        }

        [Fact]
        public async void TestConnectToken () {
            var result = await _connectController.Token (_request);
            var okResult = (OkObjectResult) result.Result;

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (new TokenResponse { }.ToString (), okResult.Value.ToString ());

            _userService.Verify (mock => mock.SetTokens (_user, "", ""), Times.Once ());
        }

        [Fact]
        public async void TestConnectToken_ValidationFailure () {

            const string errorMessage = "ERROR";

            _connectController.ModelState.AddModelError ("", errorMessage);

            var result = await _connectController.Token (new AuthenticateRequest {
                GrantType = EGrantType.client_credentials.ToString ()
            });
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedErrors = new List<string> { errorMessage };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedErrors, badResult.Value);
        }

        [Fact]
        public async void TestConnectToken_AuthenticationFailure () {

            var result = await _connectController.Token (_requestFail);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedMessage = new { message = "Username or password is incorrect" };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedMessage.ToString (), badResult.Value.ToString ());
        }

        [Fact]
        public async void TestRefreshToken () {
            var result = await _connectController.Token (_requestRefresh);
            var okResult = (OkObjectResult) result.Result;

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (new TokenResponse { }.ToString (), okResult.Value.ToString ());

            _userService.Verify (mock => mock.RemoveTokens (_user), Times.Once ());
            _userService.Verify (mock => mock.SetTokens (_user, "", ""), Times.Once ());
        }

        [Fact]
        public async void TestRefreshToken_Fail () {
            var result = await _connectController.Token (_requestRefreshFail);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedMessage = new { message = "Could not refresh token." };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedMessage.ToString (), badResult.Value.ToString ());
        }

        [Fact]
        public async void TestRefreshToken_Expired () {
            var result = await _connectController.Token (_requestRefreshExpired);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedMessage = new { message = "Refresh token expired." };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedMessage.ToString (), badResult.Value.ToString ());
        }
    }
}