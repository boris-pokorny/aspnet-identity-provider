using System.IO.Abstractions;
using AspNetIdentityApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AspNetIdentityApi.Controllers {
    [ApiController]
    [Route (".well-known")]
    public class DiscoveryController : ControllerBase {

        private readonly IConfiguration _configuration;

        private readonly IFileSystem _fileSystem;

        public DiscoveryController (IConfiguration configuration, IFileSystem fileSystem) {
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        [HttpGet ("openid-configuration")]
        public IActionResult OpenidConfiguration () {
            var applicationUrl = _configuration.GetValue<string> ("ASPNETCORE_URLS");

            var response = new OpenidConfigurationResponse {
                jwks_uri = $"{applicationUrl}/.well-known/openid-configuration/jwks",
            };
            return Ok (response);
        }

        [HttpGet ("openid-configuration/jwks")]
        public IActionResult OpenidConfigurationJwks () {
            var fileName = _configuration.GetValue<string> ("JwkFile");
            var jwk = new JsonWebKey (_fileSystem.File.ReadAllText (fileName));
            var jwkPub = new JsonWebKey {
                Kid = jwk.Kid,
                Kty = jwk.Kty,
                E = jwk.E,
                N = jwk.N,
            };
            JsonWebKeySet jsonWebKeySet = new JsonWebKeySet ();
            jsonWebKeySet.Keys.Add (jwkPub);
            return Ok (jsonWebKeySet);
        }
    }
}