using System;
using System.Threading.Tasks;
using AspNetIdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AspNetIdentityApi.Services {

    public interface IUserService {
        Task<ApplicationUser> Authenticate (AuthenticateRequest model);
        Task<int> SetTokens (ApplicationUser user, string accessToken, string refreshToken);
    }

    public class UserService : IUserService {

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        public UserService (
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
        ) {
            _signInManager = signInManager;
            _configuration = configuration;

        }

        public async Task<ApplicationUser> Authenticate (AuthenticateRequest model) {
            var result = await _signInManager.PasswordSignInAsync (model.Username, model.Password, true, lockoutOnFailure : true);
            if (result.Succeeded) {
                var userManager = _signInManager.UserManager;
                var claims = await userManager.FindByNameAsync (model.Username);
                return claims;
            }
            return null;
        }

        public async Task<int> SetTokens (ApplicationUser user, string accessToken, string refreshToken) {
            var userManager = _signInManager.UserManager;
            await userManager.SetAuthenticationTokenAsync (user, "login_provider", "access_token", accessToken);
            await userManager.SetAuthenticationTokenAsync (user, "login_provider", "refresh_token", refreshToken);
            return 0;
        }
    }
}