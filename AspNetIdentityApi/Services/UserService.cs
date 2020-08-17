using System.Linq;
using System.Threading.Tasks;
using AspNetIdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AspNetIdentityApi.Services {

    public interface IUserService {
        Task<ApplicationUser> Authenticate (AuthenticateRequest model);

        ApplicationUser HasValidRefreshToken (string refreshToken);

        Task<string> GetAccessToken (ApplicationUser user);

        Task<int> SetTokens (ApplicationUser user, string accessToken, string refreshToken);

        Task<int> RemoveTokens (ApplicationUser user);
    }

    public class UserService : IUserService {

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        private readonly string _tokenProvider = "Default";

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
            await userManager.SetAuthenticationTokenAsync (user, _tokenProvider, "access_token", accessToken);
            await userManager.SetAuthenticationTokenAsync (user, _tokenProvider, "refresh_token", refreshToken);
            return 0;
        }

        public ApplicationUser HasValidRefreshToken (string refreshToken) {
            var userManager = _signInManager.UserManager;
            var user = userManager.Users.SingleOrDefault (x => x.Tokens.SingleOrDefault (y => y.Value == refreshToken) != null);
            return user;
        }

        public async Task<string> GetAccessToken (ApplicationUser user) {
            var userManager = _signInManager.UserManager;
            var token = await userManager.GetAuthenticationTokenAsync (user, _tokenProvider, "access_token");
            return token;
        }

        public async Task<int> RemoveTokens (ApplicationUser user) {
            var userManager = _signInManager.UserManager;
            await userManager.RemoveAuthenticationTokenAsync (user, _tokenProvider, "access_token");
            await userManager.RemoveAuthenticationTokenAsync (user, _tokenProvider, "refresh_token");
            return 0;
        }
    }
}