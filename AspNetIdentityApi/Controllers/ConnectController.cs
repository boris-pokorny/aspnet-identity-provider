using System;
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
            ApplicationUser user;
            EGrantType grantType;

            if (!ModelState.IsValid) {
                return BadRequest (ModelState.GetErrorMessages ());
            }

            Enum.TryParse (model.GrantType, out grantType);

            switch (grantType) {
                case EGrantType.client_credentials:

                    user = await _userService.Authenticate (model);
                    if (user is null) {
                        return BadRequest (new { message = "Username or password is incorrect" });
                    }
                    break;

                case EGrantType.refresh_token:
                    user = _userService.HasValidRefreshToken (model.RefreshToken);
                    if (user is null) {
                        return BadRequest (new { message = "Could not refresh token." });
                    }
                    var token = await _userService.GetAccessToken (user);
                    if (_tokenService.IsRefreshExpired (token)) {
                        return BadRequest (new { message = "Refresh token expired." });
                    }
                    await _userService.RemoveTokens (user);
                    break;

                default:
                    return BadRequest ();
            }

            var accessToken = _tokenService.GenerateJwtToken (user);
            var refreshToken = _tokenService.GenerateRefreshToken ();
            var response = _tokenService.CreateResponse (user, accessToken, refreshToken);

            await _userService.SetTokens (user, accessToken, refreshToken);

            return Ok (response);
        }
    }
}