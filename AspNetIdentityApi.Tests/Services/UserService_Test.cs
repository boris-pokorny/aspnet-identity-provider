using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AspNetIdentityApi.Tests {

    public class FakeSignInManager : SignInManager<ApplicationUser> {

        public FakeSignInManager () : base (new FakeUserManager (),
            new Mock<IHttpContextAccessor> ().Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>> ().Object,
            new Mock<IOptions<IdentityOptions>> ().Object,
            new Mock<ILogger<SignInManager<ApplicationUser>>> ().Object,
            new Mock<IAuthenticationSchemeProvider> ().Object,
            new Mock<IUserConfirmation<ApplicationUser>> ().Object) { }

        override public async Task<SignInResult> PasswordSignInAsync (string a, string b, bool c, bool d) {
            return await Task.FromResult (SignInResult.Success);
        }
    }

    public class FakeSignInManagerFailure : FakeSignInManager {
        override public async Task<SignInResult> PasswordSignInAsync (string a, string b, bool c, bool d) {
            return await Task.FromResult (SignInResult.Failed);
        }
    }

    public class FakeUserManager : UserManager<ApplicationUser> {

        public FakeUserManager () : base (new Mock<IQueryableUserStore<ApplicationUser>> ().Object,
            new Mock<IOptions<IdentityOptions>> ().Object,
            new Mock<IPasswordHasher<ApplicationUser>> ().Object,
            new IUserValidator<ApplicationUser>[0],
            new IPasswordValidator<ApplicationUser>[0],
            new Mock<ILookupNormalizer> ().Object,
            new Mock<IdentityErrorDescriber> ().Object,
            new Mock<IServiceProvider> ().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>> ().Object) { }

        public override Task<IdentityResult> CreateAsync (ApplicationUser user, string password) {
            return Task.FromResult (IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync (ApplicationUser user, string role) {
            return Task.FromResult (IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync (ApplicationUser user) {
            return Task.FromResult (Guid.NewGuid ().ToString ());
        }

        public override Task<ApplicationUser> FindByNameAsync (string name) {
            return Task.FromResult (new ApplicationUser { Id = "0000-0000-0000-0000" });
        }

        public override IQueryable<ApplicationUser> Users {
            get {
                return new List<ApplicationUser> {
                    new ApplicationUser {
                        Tokens = new List<IdentityUserToken<string>> {
                            new IdentityUserToken<string> {
                                Value = "token",
                            }
                        }
                        as ICollection<IdentityUserToken<string>>
                    }
                }.AsQueryable ();
            }
        }

    }

    public class UserService_Test {
        private readonly IUserService userService;

        private readonly IUserService userServiceFail;

        public UserService_Test () {

            var signInManager = new FakeSignInManager ();
            var configuration = new Mock<IConfiguration> ();

            userService = new UserService (signInManager, configuration.Object);
            userServiceFail = new UserService (new FakeSignInManagerFailure (), configuration.Object);
        }

        [Fact]
        public async void TestAuthenticate () {
            var credentials = new AuthenticateRequest {
                Username = "",
                Password = "",
            };
            var result = await userService.Authenticate (credentials);

            Assert.IsType<ApplicationUser> (result);
        }

        [Fact]
        public async void TestAuthenticate_Failure () {
            var credentials = new AuthenticateRequest {
                Username = "",
                Password = "",
            };
            var result = await userServiceFail.Authenticate (credentials);

            Assert.Null (result);
        }

        [Fact]
        public void TestHasValidRefreshToken () {
            var result = userService.HasValidRefreshToken ("token");

            Assert.IsType<ApplicationUser> (result);
        }
    }
}