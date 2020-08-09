using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Security.Claims;
using System.Security.Cryptography;
using AspNetIdentityApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AspNetIdentityApi.Services {

    public interface ITokenService {
        string GenerateJwtToken (ApplicationUser user);

        string GenerateRefreshToken ();

        TokenResponse CreateResponse (ApplicationUser user, string accessToken, string refreshToken);
    }

    public class TokenService : ITokenService {

        private readonly IConfiguration _configuration;

        private readonly IFileSystem _fileSystem;

        private readonly int _expiresIn;

        private readonly string _jwkFile;

        public TokenService (
            IConfiguration configuration,
            IFileSystem fileSystem
        ) {
            var sectionTokens = configuration.GetSection ("Tokens");
            _configuration = configuration;
            _fileSystem = fileSystem;
            _jwkFile = sectionTokens.GetValue<string> ("JwkFile");
            _expiresIn = sectionTokens.GetValue<int> ("ExpiresIn");

        }

        public string GenerateJwtToken (ApplicationUser user) {
            var tokenHandler = new JwtSecurityTokenHandler ();
            var jwk = new JsonWebKey (_fileSystem.File.ReadAllText (_jwkFile));
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new [] { new Claim ("id", user.Id.ToString ()) }),
                Expires = DateTime.UtcNow.AddSeconds (_expiresIn),
                SigningCredentials = new SigningCredentials (jwk, SecurityAlgorithms.RsaSha256Signature),
                IssuedAt = DateTime.UtcNow,
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
        }

        public string GenerateRefreshToken () {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create ()) {
                rng.GetBytes (randomNumber);
                return Convert.ToBase64String (randomNumber);
            }
        }

        public TokenResponse CreateResponse (ApplicationUser user, string accessToken, string refreshToken) {
            var response = new TokenResponse {
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = _expiresIn,
                token_type = "Bearer"
            };
            return response;
        }
    }
}