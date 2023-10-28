using Microsoft.AspNetCore.Mvc;
using WebApi.Model;

namespace WebApi.Controllers;

[ApiController]
public class RootController : ControllerBase
{
    // GET: /.well-known/openid-configuration
    [HttpGet]
    [Route(".well-known/openid-configuration")]
    public IActionResult GetOpenIdConfiguration()
    {
        var baseUrl = Request.Host;
        var response = new OpenidConfiguration
        {
            JwksUri = $"https://{baseUrl}/oauth2/keys"
        };
        
        return Ok(response);
    }
}