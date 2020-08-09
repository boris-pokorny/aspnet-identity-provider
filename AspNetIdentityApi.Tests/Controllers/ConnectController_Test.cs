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
    }

    public class ConnectController_Test {
        private readonly ConnectController _connectController;
        private readonly ConnectController _connectControllerFail;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IUserService> _userServiceFail;
        private readonly ApplicationUser _user = new ApplicationUser { Id = "0000" };

        public ConnectController_Test () {
            var mockTokenService = new MockTokenService ();

            _userService = new Mock<IUserService> ();
            _userService.Setup (a => a.Authenticate (null)).Returns (Task.FromResult (_user));
            _userService.Setup (a => a.SetTokens (_user, "", ""));

            _userServiceFail = new Mock<IUserService> ();
            _userServiceFail.Setup (a => a.Authenticate (null)).Returns (Task.FromResult<ApplicationUser> (null));

            _connectController = new ConnectController (null, _userService.Object, mockTokenService);
            _connectControllerFail = new ConnectController (null, _userServiceFail.Object, mockTokenService);
        }

        [Fact]
        public async void TestConnectToken () {
            var result = await _connectController.Token (null);
            var okResult = (OkObjectResult) result.Result;

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (new TokenResponse { }.ToString (), okResult.Value.ToString ());

            _userService.Verify (mock => mock.SetTokens (_user, "", ""), Times.Once ());
        }

        [Fact]
        public async void TestConnectToken_ValidationFailure () {

            const string errorMessage = "ERROR";

            _connectController.ModelState.AddModelError ("", errorMessage);

            var result = await _connectController.Token (null);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedErrors = new List<string> { errorMessage };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedErrors, badResult.Value);
        }

        [Fact]
        public async void TestConnectToken_AuthenticationFailure () {

            var result = await _connectControllerFail.Token (null);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedMessage = new { message = "Username or password is incorrect" };

            Assert.IsType<ActionResult<TokenResponse>> (result);
            Assert.Equal (expectedMessage.ToString (), badResult.Value.ToString ());
        }
    }
}