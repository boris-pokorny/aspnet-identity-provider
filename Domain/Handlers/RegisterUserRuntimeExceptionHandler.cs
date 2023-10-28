using Domain.Commands;
using Domain.Exceptions;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class RegisterUserRuntimeExceptionHandler  : IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, RuntimeException>
{
    private readonly ILogger<RegisterUserRuntimeExceptionHandler> _logger;
    
    public RegisterUserRuntimeExceptionHandler(
        ILogger<RegisterUserRuntimeExceptionHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(
        RegisterUserCommand request,
        RuntimeException exception,
        RequestExceptionHandlerState<RegisterUserResponse> state,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception.Message);
        
        state.SetHandled(new RegisterUserResponse()
        {
            Status = RegisterUserStatus.Failure,
            Error = exception.Message
        });
    }
}