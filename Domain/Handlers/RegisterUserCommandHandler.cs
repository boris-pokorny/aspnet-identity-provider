using Domain.Commands;
using Domain.MessageBus;
using Domain.Model;
using Domain.Ports;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers;

public class RegisterUserCommandHandler : IMessageHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;

    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ILogger<RegisterUserCommandHandler> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"{nameof(Handle)}(${command})");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = command.UserName
        };

        _ = await _userRepository.AddUserAsync(user, command.Password);

        var response = new RegisterUserResponse
        {
            Status = RegisterUserStatus.Success
        };
        
        _logger.LogDebug($"{nameof(Handle)} -> {response}");

        return response;
    }
}