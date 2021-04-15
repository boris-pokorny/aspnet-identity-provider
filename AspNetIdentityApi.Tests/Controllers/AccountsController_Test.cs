using System.Collections.Generic;
using AspNetIdentityApi.Controllers;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class AccountsController_Test {
        private readonly AccountsController _accountsController;
        private readonly RegistrationRequest _registrationRequest = new RegistrationRequest { 
            Email = "a@a",
            FirstName = "a",
            LastName = "b",
            UserName = "c"
        };
        private readonly Mock<IUserService> _userService = new Mock<IUserService> ();
        private readonly Mock<ITokenService> _tokenService = new Mock<ITokenService> ();
        public AccountsController_Test () {
            _userService
                .Setup (a => a.CreateAccount (It.IsAny<ApplicationUser> (), It.IsAny<string> ()))
                .Callback<ApplicationUser, string> ((user, password) => user.Id = "0000");
            _accountsController = new AccountsController (null, _userService.Object, _tokenService.Object);
        }

        [Fact]
        public async void TestRegistrationSuccess () {
            var result = await _accountsController.Registration (_registrationRequest);
            var createdResult = (CreatedResult) result;
            var user = (ApplicationUser) result.Value;
            Assert.Equal ("/accounts/0000", createdResult.Location);
            Assert.Equal (_registrationRequest.Email, user.Email);
            Assert.Equal ("a", user.FirstName);
            Assert.Equal ("b", user.LastName);
            Assert.Equal ("c", user.UserName);
        }

        [Fact]
        public async void TestRegistrationFailure () {

            const string errorMessage = "ERROR";

            _accountsController.ModelState.AddModelError ("", errorMessage);

            var result = await _accountsController.Registration (_registrationRequest);
            var badResult = (BadRequestObjectResult) result;
            var expectedErrors = new List<string> { errorMessage };

            Assert.IsType<BadRequestObjectResult> (result);
            Assert.Equal (expectedErrors, badResult.Value);
        }
    }
}