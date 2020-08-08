using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetIdentityApi.Controllers;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public interface IError {
        string message { get; set; }
    }

    public class MockUserService : IUserService {
        public const string token = "TOKEN";
        public async Task<string> Authenticate (AuthenticateRequest model) {
            return await Task.FromResult<string> (token);
        }
    }

    public class MockUserServiceInvalid : IUserService {
        public async Task<string> Authenticate (AuthenticateRequest model) {
            return await Task.FromResult<string> (null);
        }
    }

    public class ConnectController_Test {
        private readonly ConnectController connectController;
        private readonly ConnectController connectControllerInvalid;

        public ConnectController_Test () {
            var mockUserService = new MockUserService ();
            var mockUserServiceInvalid = new MockUserServiceInvalid ();
            connectController = new ConnectController (null, mockUserService);
            connectControllerInvalid = new ConnectController (null, mockUserServiceInvalid);
        }

        [Fact]
        public async void TestConnectToken () {
            var result = await connectController.Token (null);
            var okResult = (OkObjectResult) result.Result;

            Assert.IsType<ActionResult<string>> (result);
            Assert.Equal (MockUserService.token, okResult.Value);
        }

        [Fact]
        public async void TestConnectToken_ValidationFailure () {

            const string errorMessage = "ERROR";

            connectController.ModelState.AddModelError ("", errorMessage);

            var result = await connectController.Token (null);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedErrors = new List<string> { errorMessage };

            Assert.IsType<ActionResult<string>> (result);
            Assert.Equal (expectedErrors, badResult.Value);
        }

        [Fact]
        public async void TestConnectToken_AuthenticationFailure () {

            var result = await connectControllerInvalid.Token (null);
            var badResult = (BadRequestObjectResult) result.Result;
            var expectedMessage = new { message = "Username or password is incorrect" };

            Assert.IsType<ActionResult<string>> (result);
            Assert.Equal (expectedMessage.ToString (), badResult.Value.ToString ());
        }
    }
}