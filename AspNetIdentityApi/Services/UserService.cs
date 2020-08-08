using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetIdentityApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AspNetIdentityApi.Services {

    public interface IUserService {
        Task<string> Authenticate (AuthenticateRequest model);
    }

    public class UserService : IUserService {

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        private readonly IFileSystem _fileSystem;

        public UserService (
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IFileSystem fileSystem
        ) {
            _signInManager = signInManager;
            _configuration = configuration;
            _fileSystem = fileSystem;

        }

        public async Task<string> Authenticate (AuthenticateRequest model) {
            var result = await _signInManager.PasswordSignInAsync (model.Username, model.Password, true, lockoutOnFailure : true);
            if (result.Succeeded) {
                var userManager = _signInManager.UserManager;
                var claims = await userManager.FindByNameAsync (model.Username);
                var token = generateJwtToken (claims);
                await userManager.SetAuthenticationTokenAsync (claims, "login_provider", "access_token", token);
                return token;
            }
            return null;
        }

        private string generateJwtToken (ApplicationUser user) {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler ();
            var fileName = _configuration.GetValue<string> ("JwkFile");
            var jwk = new JsonWebKey (_fileSystem.File.ReadAllText (fileName));
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new [] { new Claim ("id", user.Id.ToString ()) }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (jwk, SecurityAlgorithms.RsaSha256Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
        }

    }
}