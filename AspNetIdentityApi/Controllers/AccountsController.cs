using System;
using System.Threading.Tasks;
using AspNetIdentityApi.Extensions;
using AspNetIdentityApi.Models;
using AspNetIdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetIdentityApi.Controllers {
    /// Accounts  Controller
    [Route ("[controller]")]
    public class AccountsController : ControllerBase {

        private readonly ILogger<AccountsController> _logger;

        private readonly IUserService _userService;

        private readonly ITokenService _tokenService;

        public AccountsController (
            ILogger<AccountsController> logger,
            IUserService userService,
            ITokenService tokenService
        ) {
            _logger = logger;
            _userService = userService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Creates an account.
        /// </summary>
        [HttpPost ("Registration")]
        public async Task<ObjectResult> Registration ([FromBody] RegistrationRequest model) {

            if (!ModelState.IsValid) {
                return BadRequest (ModelState.GetErrorMessages ());
            }

            var user = new ApplicationUser {
                Email = model.Email,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            await _userService.CreateAccount (user, model.Password);

            return Created ($"/accounts/{user.Id}", user);
        }
    }
}