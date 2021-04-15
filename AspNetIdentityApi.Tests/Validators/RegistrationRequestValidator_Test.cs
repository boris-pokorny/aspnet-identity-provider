using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class MockUserManager : ApplicationUserManager {
        public MockUserManager (
            List<IUserValidator<ApplicationUser>> userValidators,
            List<IPasswordValidator<ApplicationUser>> passwordValidators
        ) : base (
            new Mock<IUserEmailStore<ApplicationUser>> ().Object,
            new Mock<IOptions<IdentityOptions>> ().Object,
            new Mock<IPasswordHasher<ApplicationUser>> ().Object,
            userValidators,
            passwordValidators,
            new Mock<ILookupNormalizer> ().Object,
            new Mock<IdentityErrorDescriber> ().Object,
            new Mock<IServiceProvider> ().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>> ().Object
        ) { }
    }

    public class RegistrationRequestValidator_Test {
        private readonly RegistrationRequestValidator _validator;
        private readonly Mock<IPasswordValidator<ApplicationUser>> _passwordValidator = new Mock<IPasswordValidator<ApplicationUser>> ();
        private readonly Mock<IUserValidator<ApplicationUser>> _userValidator = new Mock<IUserValidator<ApplicationUser>> ();
        private readonly Mock<MockUserManager> _userManager;
        private readonly RegistrationRequest _request = new RegistrationRequest {
            UserName = "a",
            FirstName = "a",
            LastName = "a",
            Email = "a",
            Password = "a",
            ConfirmPassword = "a"
        };

        public RegistrationRequestValidator_Test () {
            _userManager = new Mock<MockUserManager> (
                new List<IUserValidator<ApplicationUser>> { _userValidator.Object },
                new List<IPasswordValidator<ApplicationUser>> { _passwordValidator.Object }
            );
            _validator = new RegistrationRequestValidator (_userManager.Object);
        }

        [Fact]
        public void TestValid () {
            _userValidator.Setup (a => a.ValidateAsync (_userManager.Object, It.IsAny<ApplicationUser> ()))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, _request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            var result = _validator.Validate (_request);

            Assert.True (result.IsValid);
        }

        [Fact]
        public void TestInvalid () {
            var request = new RegistrationRequest { };

            _userValidator.Setup (a => a.ValidateAsync (_userManager.Object, It.IsAny<ApplicationUser> ()))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (6, result.Errors.Count);
            Assert.Equal ("'First Name' must not be empty.", result.Errors[0].ErrorMessage);
            Assert.Equal ("'Last Name' must not be empty.", result.Errors[1].ErrorMessage);
            Assert.Equal ("'User Name' must not be empty.", result.Errors[2].ErrorMessage);
            Assert.Equal ("'Email' must not be empty.", result.Errors[3].ErrorMessage);
            Assert.Equal ("'Password' must not be empty.", result.Errors[4].ErrorMessage);
            Assert.Equal ("'Confirm Password' must not be empty.", result.Errors[5].ErrorMessage);
        }

        [Fact]
        public void TestConfirmPasswordInvalid () {
            var request = _request;
            request.Password = "x";
            _userValidator.Setup (a => a.ValidateAsync (_userManager.Object, It.IsAny<ApplicationUser> ()))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ("'Confirm Password' and 'Password' do not match.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestEmailExists () {
            var request = _request;
            _userValidator.Setup (a => a.ValidateAsync (_userManager.Object, It.IsAny<ApplicationUser> ()))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _userManager.Setup (a => a.FindByEmailAsync (request.Email))
                .Returns (Task.FromResult<ApplicationUser> (new ApplicationUser { }));

            var result = _validator.Validate (request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ($"An account with email {request.Email} already exists.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestPasswordRules () {
            _userValidator.Setup (a => a.ValidateAsync (_userManager.Object, It.IsAny<ApplicationUser> ()))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, _request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Failed (new IdentityError ())));

            var result = _validator.Validate (_request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ($"'Password' does not comply with password rules.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TestUserNameExists () {
            _userManager.Setup (a => a.FindByNameAsync (It.IsAny<string> ()))
                .Returns (Task.FromResult<ApplicationUser> (new ApplicationUser { }));

            _passwordValidator.Setup (a => a.ValidateAsync (_userManager.Object, null, _request.Password))
                .Returns (Task.FromResult<IdentityResult> (IdentityResult.Success));

            var result = _validator.Validate (_request);

            Assert.False (result.IsValid);
            Assert.Equal (1, result.Errors.Count);
            Assert.Equal ($"User Name {_request.UserName} already taken.", result.Errors[0].ErrorMessage);
        }
    }
}