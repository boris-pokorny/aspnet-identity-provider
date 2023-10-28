using Domain.Commands;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class AuthenticateExceptionHandler : IRequestExceptionHandler<AuthenticateCommand, AuthenticateResponse, Exception>
{
    private readonly ILogger<AuthenticateExceptionHandler> _logger;

    public AuthenticateExceptionHandler(
        ILogger<AuthenticateExceptionHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(
        AuthenticateCommand request,
        Exception exception,
        RequestExceptionHandlerState<AuthenticateResponse> state,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception.Message);

        state.SetHandled(new AuthenticateResponse()
        {
            Status = AuthenticateStatus.Failure
        });
    }
}