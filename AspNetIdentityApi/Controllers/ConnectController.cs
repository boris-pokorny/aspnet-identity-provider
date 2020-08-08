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
        public ConnectController (
            ILogger<ConnectController> logger,
            IUserService userService) {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost ("Token")]
        public async Task<ActionResult<string>> Token (AuthenticateRequest model) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState.GetErrorMessages ());
            }
            var token = await _userService.Authenticate (model);
            if (string.IsNullOrEmpty (token)) {
                return BadRequest (new { message = "Username or password is incorrect" });
            }
            return Ok (token);

        }
    }
}