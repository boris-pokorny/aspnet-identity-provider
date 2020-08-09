using System.Threading.Tasks;
using AspNetIdentityApi.Extensions;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetIdentityApi.Controllers {
    [ApiController]
    [Route ("[controller]")]
    public class ConnectController : ControllerBase {

        private readonly ILogger<ConnectController> _logger;

        private readonly IUserService _userService;

        private readonly ITokenService _tokenService;

        public ConnectController (
            ILogger<ConnectController> logger,
            IUserService userService,
            ITokenService tokenService
        ) {
            _logger = logger;
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost ("Token")]
        public async Task<ActionResult<TokenResponse>> Token (AuthenticateRequest model) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState.GetErrorMessages ());
            }
            var user = await _userService.Authenticate (model);
            if (user is null) {
                return BadRequest (new { message = "Username or password is incorrect" });
            }
            var accessToken = _tokenService.GenerateJwtToken (user);
            var refreshToken = _tokenService.GenerateRefreshToken ();
            var response = _tokenService.CreateResponse (user, accessToken, refreshToken);

            await _userService.SetTokens (user, accessToken, refreshToken);

            return Ok (response);
        }
    }
}