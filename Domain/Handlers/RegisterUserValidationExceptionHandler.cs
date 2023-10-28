using Domain.Commands;
using FluentValidation;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class RegisterUserValidationExceptionHandler : IRequestExceptionHandler<RegisterUserCommand, RegisterUserResponse, ValidationException>
{
    private readonly ILogger<RegisterUserValidationExceptionHandler> _logger;

    public RegisterUserValidationExceptionHandler(
        ILogger<RegisterUserValidationExceptionHandler> logger
    )
    {
        _logger = logger;
    }

    public async Task Handle(
        RegisterUserCommand request,
        ValidationException exception,
        RequestExceptionHandlerState<RegisterUserResponse> state,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception.Message);
        
        state.SetHandled(new RegisterUserResponse()
        {
            Error = exception.Message
        });
    }
}