using Domain.Commands;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("oauth2")]
public class OAuth2Controller : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OAuth2Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: /register
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserCommand command)
    {
        var response = await _mediator.Send(command);
        var responseToResult = new Dictionary<RegisterUserStatus, ObjectResult>()
        {
            [RegisterUserStatus.Success] = Ok(response),
            [RegisterUserStatus.Failure] = BadRequest(response.Error),
        };
        var result = responseToResult[response.Status];

        return result;
    }
    
    // POST: /token
    [HttpPost]
    [Route("token")]
    public async Task<IActionResult> Authenticate(AuthenticateCommand command)
    {
        var response = await _mediator.Send(command);
        var responseToResult = new Dictionary<AuthenticateStatus, ObjectResult>()
        {
            [AuthenticateStatus.Success] = Ok(response),
            [AuthenticateStatus.Failure] = BadRequest("Authentication failed."),
        };
        var result = responseToResult[response.Status];

        return result;
    }
    
    // GET: /keys
    [HttpGet]
    [Route("keys")]
    public async Task<IActionResult> GetJsonWebKeySet()
    {
        var query = new GetJsonWebKeySetQuery();
        var response = await _mediator.Send(query);
        var responseToResult = new Dictionary<GetJsonWebKeySetStatus, ObjectResult>()
        {
            [GetJsonWebKeySetStatus.Success] = Ok(response),
            [GetJsonWebKeySetStatus.Failure] = NotFound("Json web key set not found."),
        };
        var result = responseToResult[response.Status];

        return result;
    }
}
